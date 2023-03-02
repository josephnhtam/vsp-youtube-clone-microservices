import {createAction, props} from '@ngrx/store';
import {ChannelSection} from 'src/app/core/models/channel';

export const instantiate = createAction(
  '[channel sections] instantiate sections',
  props<{
    userId: string;
    sections: ChannelSection[];
    contextId: string;
  }>()
);

export const resetChannel = createAction(
  '[channel section] reset channel',
  props<{ userId: string }>()
);
