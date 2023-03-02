import {BehaviorSubject} from 'rxjs';
import * as SignalR from '@microsoft/signalr';

// HubConnection wrapper with connection retry
export class HubClient {
  private _connection: SignalR.HubConnection;
  private _stopPromise: Promise<void> | null = null;
  private _retryPolicy: SignalR.IRetryPolicy;
  private _stateSubject = new BehaviorSubject<HubClientState>(
    HubClientState.Disconnected
  );

  get state$() {
    return this._stateSubject.asObservable();
  }

  get state() {
    return this._stateSubject.value;
  }

  get connectionId() {
    return this._connection.connectionId;
  }

  get serverTimeoutInMilliseconds() {
    return this._connection.serverTimeoutInMilliseconds;
  }

  get keepAliveIntervalInMilliseconds() {
    return this._connection.keepAliveIntervalInMilliseconds;
  }

  constructor(options: HubClientOptions) {
    const builder = new SignalR.HubConnectionBuilder();

    if (!!options.httpConnectionOptions) {
      builder.withUrl(options.hubUrl, options.httpConnectionOptions);
    } else {
      builder.withUrl(options.hubUrl);
    }

    if (options.configureBuilder) {
      options.configureBuilder(builder);
    }

    if (options.retryPolicy) {
      this._retryPolicy = options.retryPolicy;
    } else {
      this._retryPolicy = this.getDefaultRetryPolicy();
    }

    builder.withAutomaticReconnect(this._retryPolicy);

    this._connection = builder.build();

    this._connection.onclose(this.onclose.bind(this));
    this._connection.onreconnected(this.onreconnected.bind(this));
    this._connection.onreconnecting(this.onreconnecting.bind(this));

    if (options.serverTimeoutInMilliseconds) {
      this._connection.serverTimeoutInMilliseconds =
        options.serverTimeoutInMilliseconds;
    }

    if (options.keepAliveIntervalInMilliseconds) {
      this._connection.keepAliveIntervalInMilliseconds =
        options.keepAliveIntervalInMilliseconds;
    }
  }

  async stop() {
    if (this.state == HubClientState.Disconnected) return;

    if (this._stopPromise != null) {
      await this._stopPromise;
      return;
    }

    this._stopPromise = this.doStop();
    await this._stopPromise;
  }

  private async doStop() {
    this._stateSubject.next(HubClientState.Disconnecting);
    await this._connection.stop();
    this._stateSubject.next(HubClientState.Disconnected);
    this._stopPromise = null;
  }

  async start() {
    if (this._connection.state !== SignalR.HubConnectionState.Disconnected) {
      throw new Error(
        "Cannot start a HubConnection that is not in the 'Disconnected' state."
      );
    }

    this._stateSubject.next(HubClientState.Connecting);

    const startTime = Date.now();

    let retryCount = 0;
    while (this._stateSubject.value === HubClientState.Connecting) {
      try {
        await this._connection.start();
        this._stateSubject.next(HubClientState.Connected);

        console.log(`Connected to Hub ${this._connection.baseUrl}.`);
      } catch (error: any) {
        if (this._stateSubject.value === HubClientState.Connecting) {
          const retryContext: SignalR.RetryContext = {
            elapsedMilliseconds: Date.now() - startTime,
            previousRetryCount: retryCount,
            retryReason: error,
          };

          const retryDelay =
            this._retryPolicy.nextRetryDelayInMilliseconds(retryContext);

          if (retryDelay != null) {
            console.log(`Retrying connection in ${retryDelay / 1000} seconds.`);
            await this.delay(retryDelay);
          } else {
            console.log(`Retry is rejected.`);
            this._stateSubject.next(HubClientState.Disconnected);
            throw error;
          }

          retryCount++;
        } else {
          throw error;
        }
      }
    }
  }

  private getDefaultRetryPolicy(): SignalR.IRetryPolicy {
    return {
      nextRetryDelayInMilliseconds: (context) => {
        return Math.min(32, Math.pow(2, context.previousRetryCount)) * 1000;
      },
    };
  }

  async delay(delaySeconds: number) {
    return new Promise<void>((resolve) => {
      setTimeout(resolve, delaySeconds);
    });
  }

  /** Invokes a streaming hub method on the server using the specified name and arguments.
   *
   * @typeparam T The type of the items returned by the server.
   * @param {string} methodName The name of the server method to invoke.
   * @param {any[]} args The arguments used to invoke the server method.
   * @returns {IStreamResult<T>} An object that yields results from the server as they are received.
   */
  stream<T = any>(
    methodName: string,
    ...args: any[]
  ): SignalR.IStreamResult<T> {
    return this._connection.stream(methodName, ...args);
  }

  /** Invokes a hub method on the server using the specified name and arguments. Does not wait for a response from the receiver.
   *
   * The Promise returned by this method resolves when the client has sent the invocation to the server. The server may still
   * be processing the invocation.
   *
   * @param {string} methodName The name of the server method to invoke.
   * @param {any[]} args The arguments used to invoke the server method.
   * @returns {Promise<void>} A Promise that resolves when the invocation has been successfully sent, or rejects with an error.
   */
  send(methodName: string, ...args: any[]): Promise<void> {
    return this._connection.send(methodName, ...args);
  }

  /** Invokes a hub method on the server using the specified name and arguments.
   *
   * The Promise returned by this method resolves when the server indicates it has finished invoking the method. When the promise
   * resolves, the server has finished invoking the method. If the server method returns a result, it is produced as the result of
   * resolving the Promise.
   *
   * @typeparam T The expected return type.
   * @param {string} methodName The name of the server method to invoke.
   * @param {any[]} args The arguments used to invoke the server method.
   * @returns {Promise<T>} A Promise that resolves with the result of the server method (if any), or rejects with an error.
   */
  invoke<T = any>(methodName: string, ...args: any[]): Promise<T> {
    return this._connection.invoke(methodName, ...args);
  }

  /** Registers a handler that will be invoked when the hub method with the specified method name is invoked.
   *
   * @param {string} methodName The name of the hub method to define.
   * @param {Function} newMethod The handler that will be raised when the hub method is invoked.
   */
  on(methodName: string, newMethod: (...args: any[]) => any): void {
    return this._connection.on(methodName, newMethod);
  }

  /** Removes all handlers for the specified hub method.
   *
   * @param {string} methodName The name of the method to remove handlers for.
   */
  off(methodName: string): void;
  /** Removes the specified handler for the specified hub method.
   *
   * You must pass the exact same Function instance as was previously passed to {@link @microsoft/signalr.HubConnection.on}. Passing a different instance (even if the function
   * body is the same) will not remove the handler.
   *
   * @param {string} methodName The name of the method to remove handlers for.
   * @param {Function} method The handler to remove. This must be the same Function instance as the one passed to {@link @microsoft/signalr.HubConnection.on}.
   */
  off(methodName: string, method: (...args: any[]) => void): void;
  off(methodName: string, method?: (...args: any[]) => void): void {
    if (method) {
      this._connection.off(methodName, method);
    } else {
      this._connection.off(methodName);
    }
  }

  onclose(error?: Error): void {
    this._stateSubject.next(HubClientState.Disconnected);
  }

  onreconnecting(error?: Error): void {
    this._stateSubject.next(HubClientState.Reconnecting);
  }

  onreconnected(connectionId?: string): void {
    this._stateSubject.next(HubClientState.Connected);
  }
}

export interface HubClientOptions {
  hubUrl: string;
  httpConnectionOptions?: SignalR.IHttpConnectionOptions;
  configureBuilder?: (builder: SignalR.HubConnectionBuilder) => void;
  retryPolicy?: SignalR.IRetryPolicy;
  serverTimeoutInMilliseconds?: number;
  keepAliveIntervalInMilliseconds?: number;
}

export enum HubClientState {
  Disconnected = 'Disconnected',
  Connecting = 'Connecting',
  Connected = 'Connected',
  Disconnecting = 'Disconnecting',
  Reconnecting = 'Reconnecting',
}
