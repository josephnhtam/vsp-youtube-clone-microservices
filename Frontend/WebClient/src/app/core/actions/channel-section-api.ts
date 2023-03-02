import {ChannelSectionInstance} from '../models/channel';
import {createAction, props} from '@ngrx/store';
import {ChannelSection} from 'src/app/core/models/channel';

export const instantiated = createAction(
  '[channel sections / api ] sections instantiated',
  props<{
    userId: string;
    contextId: string;
    sections: ChannelSection[];
    sectionInstances: ChannelSectionInstance[];
  }>()
);
