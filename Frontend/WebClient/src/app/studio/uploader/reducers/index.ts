import {createEntityAdapter, EntityState} from '@ngrx/entity';
import {UploadProcess, UploadStatus} from '../models/index';
import {createReducer, on} from '@ngrx/store';
import {UploaderApiAction} from '../actions';

export const featureKey = 'uploader';

export const adapter = createEntityAdapter<UploadProcess>({
  selectId: (x) => x.request.uploadToken,
});

export interface State extends EntityState<UploadProcess> {}

const initialState: State = adapter.getInitialState();

export const reducer = createReducer<State>(
  initialState,

  on(UploaderApiAction.uploadCancelled, (state, { uploadToken }) => {
    return adapter.updateOne(
      {
        id: uploadToken,
        changes: {
          status: UploadStatus.Cancelled,
        },
      },
      {
        ...state,
      }
    );
  }),

  on(UploaderApiAction.uploadProcessCreated, (state, { uploadProcess }) => {
    return adapter.addOne(uploadProcess, state);
  }),

  on(
    UploaderApiAction.uploadProgressUpdated,
    (state, { uploadToken, progress }) => {
      return adapter.updateOne(
        {
          id: uploadToken,
          changes: {
            progress,
          },
        },
        state
      );
    }
  ),

  on(UploaderApiAction.uploadSuccessful, (state, { uploadToken }) => {
    return adapter.updateOne(
      {
        id: uploadToken,
        changes: {
          status: UploadStatus.Successful,
        },
      },
      state
    );
  }),

  on(UploaderApiAction.uploadFailed, (state, { uploadToken }) => {
    return adapter.updateOne(
      {
        id: uploadToken,
        changes: {
          status: UploadStatus.Failed,
        },
      },
      state
    );
  })
);
