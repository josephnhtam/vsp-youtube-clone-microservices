import {ChannelSectionInstance} from '../models/channel';
import {selectChannelSectionsState} from './index';
import {createSelector} from '@ngrx/store';
import {adapter} from '../reducers/channel-sections';
import {ChannelSectionType} from 'src/app/core/models/channel';

const selectors = adapter.getSelectors();

export const selectChannel = (id: string) =>
  createSelector(selectChannelSectionsState, (state) =>
    selectors.selectAll(state).find((x) => x.id == id)
  );

export const selectChannelSectionInstance = (id: string, sectionId: string) =>
  createSelector(selectChannel(id), (channel) =>
    channel?.sectionInstances.find((x) => x.id == sectionId)
  );

export const selectChannelSectionInstances = (
  id: string,
  sectionIds: string[]
) =>
  createSelector(selectChannel(id), (channel) => {
    return sectionIds
      .map((id) => channel?.sectionInstances.find((x) => x.id == id))
      .filter((section): section is ChannelSectionInstance => {
        return !!section;
      });
  });

export const selectChannelSectionInstanceByType = (
  id: string,
  type: ChannelSectionType
) =>
  createSelector(selectChannel(id), (channel) =>
    channel?.sectionInstances.find((x) => x.type == type)
  );

export const selectIsChannelSectionsLoading = (id: string, contextId: string) =>
  createSelector(selectChannel(id), (channel) => {
    return channel?.loadingContextIds.some((x) => x == contextId);
  });
