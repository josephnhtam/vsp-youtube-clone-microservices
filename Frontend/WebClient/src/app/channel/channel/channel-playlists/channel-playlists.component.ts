import {ChannelSectionType, MultiplePlaylistsSection, UserChannel,} from '../../../core/models/channel';
import {Component, OnDestroy, OnInit} from '@angular/core';
import {ActivatedRoute} from '@angular/router';
import {Subscription} from 'rxjs';

@Component({
  selector: 'app-channel-playlists',
  templateUrl: './channel-playlists.component.html',
  styleUrls: ['./channel-playlists.component.css'],
})
export class ChannelPlaylistsComponent implements OnInit, OnDestroy {
  channel!: UserChannel;
  view: ViewTarget = ViewTarget.Overview;
  targetSection?: MultiplePlaylistsSection;

  private initialized = false;
  private querySub?: Subscription;
  private dataSub?: Subscription;

  constructor(private route: ActivatedRoute) {
    this.dataSub = this.route.data.subscribe(this.onRefreshed.bind(this));

    this.querySub = this.route.queryParams.subscribe(
      this.onRefreshed.bind(this)
    );
  }

  ngOnDestroy(): void {
    this.dataSub?.unsubscribe();
    this.querySub?.unsubscribe();
  }

  ngOnInit(): void {
    this.initialized = true;
    this.onRefreshed();
  }

  onRefreshed() {
    if (!this.initialized) return;
    const route = this.route.snapshot;

    this.channel = route.data['playlistsChannel'] as UserChannel;

    const queryParams = route.queryParams;
    const viewString = (queryParams['view'] as string) ?? '';

    if (this.channel.sections.length == 1) {
      const section = this.channel.sections[0];

      if (section.type == ChannelSectionType.CreatedPlaylists) {
        this.refresh(ViewTarget.Created);
      } else {
        this.refresh(ViewTarget.Section, section as MultiplePlaylistsSection);
      }
    } else if (viewString == '' || viewString.toLowerCase() == 'overview') {
      this.refresh(ViewTarget.Overview);
    } else if (viewString.toLowerCase() == 'created') {
      this.refresh(ViewTarget.Created);
    } else {
      const section = this.channel.sections.find(
        (x) => x.id === viewString
      ) as MultiplePlaylistsSection;

      if (!!section) {
        this.refresh(ViewTarget.Section, section);
      } else {
        this.refresh(ViewTarget.Overview);
      }
    }
  }

  refresh(view: ViewTarget, section?: MultiplePlaylistsSection) {
    this.view = view;
    this.targetSection = section;
  }
}

export enum ViewTarget {
  Overview,
  Created,
  Section,
}
