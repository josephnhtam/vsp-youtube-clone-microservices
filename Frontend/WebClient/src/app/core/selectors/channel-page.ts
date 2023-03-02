import {createSelector} from '@ngrx/store';
import {adapter} from '../reducers/channel-page';
import {selectChannelPageState} from "./index";

const selectors = adapter.getSelectors();

export const selectChannelPages = createSelector(
  selectChannelPageState,
  (state) => selectors.selectAll(state)
);

export const selectChannelPage = (userId: string, contextId: string) =>
  createSelector(selectChannelPages, (pages) =>
    pages.find((x) => x.userId === userId && x.contextId === contextId)
  );

export const selectHasMoreChannelPageItems = (
  userId: string,
  contextId: string
) =>
  createSelector(selectChannelPage(userId, contextId), (page) => {
    if (!page) return false;
    return (
      page.loadedLastItem ||
      (page.content?.totalCount ?? 0) > (page.content?.items.length ?? 0)
    );
  });
