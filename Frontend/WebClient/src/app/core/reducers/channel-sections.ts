import {ChannelSectionInstance} from '../models/channel';
import {createEntityAdapter, EntityState} from '@ngrx/entity';
import {createReducer, on} from '@ngrx/store';
import {ChannelSectionAction, ChannelSectionApiAction} from '../actions';

export interface Channel {
  id: string;
  sectionInstances: ChannelSectionInstance[];
  loadingContextIds: string[];
}

export interface State extends EntityState<Channel> {}

export const adapter = createEntityAdapter<Channel>({
  selectId: (x) => x.id,
});

const initialState: State = adapter.getInitialState();

export const reducer = createReducer(
  initialState,

  on(
    ChannelSectionApiAction.instantiated,
    (state, { userId, contextId, sectionInstances }) => {
      const channel = state.entities[userId]!;

      let updatedSectionInstances = channel.sectionInstances.filter(
        (instance) => !sectionInstances.some((x) => x.id == instance.id)
      );

      for (let instance of sectionInstances) {
        if (!updatedSectionInstances.some((x) => x.id == instance.id)) {
          updatedSectionInstances.push(instance);
        }
      }

      return adapter.upsertOne(
        {
          ...channel,
          sectionInstances: updatedSectionInstances,
          loadingContextIds: channel.loadingContextIds.filter(
            (x) => x != contextId
          ),
        },
        { ...state }
      );
    }
  ),

  on(ChannelSectionAction.instantiate, (state, { userId, contextId }) => {
    const existingChannel = state.entities[userId];
    if (!!existingChannel) {
      return adapter.upsertOne(
        {
          ...existingChannel,
          loadingContextIds: [...existingChannel.loadingContextIds, contextId],
        },
        {
          ...state,
        }
      );
    } else {
      return adapter.addOne(
        {
          id: userId,
          sectionInstances: [],
          loadingContextIds: [contextId],
        },
        {
          ...state,
        }
      );
    }
  })
);
