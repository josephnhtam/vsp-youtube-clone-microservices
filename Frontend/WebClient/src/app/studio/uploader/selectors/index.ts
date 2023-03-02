import {createSelector} from '@ngrx/store';
import {selectUploaderState} from '../../selectors';
import {adapter} from '../reducers';

const selectors = adapter.getSelectors();

export const selectUploadProcesses = createSelector(
  selectUploaderState,
  (state) => selectors.selectAll(state)
);

export const selectUploadProcess = (uploadToken: string) =>
  createSelector(selectUploadProcesses, (state) =>
    state.find((p) => p.request.uploadToken === uploadToken)
  );
