import {UserChannel,} from '../../../../core/models/channel';
import {Component, Input, OnInit} from '@angular/core';
import {of, Subscription} from 'rxjs';
import {Store} from '@ngrx/store';
import {selectChannelSectionInstances, selectIsChannelSectionsLoading,} from 'src/app/core/selectors/channel-sections';
import {Guid} from 'guid-typescript';
import {ChannelSectionAction} from 'src/app/core/actions';

@Component({
  selector: 'app-channel-playlists-overview',
  templateUrl: './channel-playlists-overview.component.html',
  styleUrls: ['./channel-playlists-overview.component.css'],
})
export class ChannelPlaylistsOverviewComponent implements OnInit {
  @Input() channel!: UserChannel;

  private contextId?: string;
  private channelSub?: Subscription;

  constructor(private store: Store) {}

  ngOnDestroy(): void {
    this.channelSub?.unsubscribe();
  }

  ngOnInit(): void {
    this.contextId = Guid.create().toString();

    this.store.dispatch(
      ChannelSectionAction.instantiate({
        userId: this.channel.id,
        sections: this.channel.sections,
        contextId: this.contextId,
      })
    );
  }

  get pending$() {
    if (this.contextId == null) {
      return of(true);
    }

    return this.store.select(
      selectIsChannelSectionsLoading(this.channel.id, this.contextId)
    );
  }

  get sections$() {
    return this.store.select(
      selectChannelSectionInstances(
        this.channel.id,
        this.channel.sections.map((x) => x.id).filter((x): x is string => !!x)
      )
    );
  }
}
