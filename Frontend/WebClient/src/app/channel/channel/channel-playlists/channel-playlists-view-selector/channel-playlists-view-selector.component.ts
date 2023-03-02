import {Router} from '@angular/router';
import {Component, Input, OnChanges, OnInit, SimpleChanges,} from '@angular/core';
import {ChannelSectionType, MultiplePlaylistsSection, UserChannel,} from 'src/app/core/models/channel';
import {ViewTarget} from '../channel-playlists.component';

@Component({
  selector: 'app-channel-playlists-view-selector',
  templateUrl: './channel-playlists-view-selector.component.html',
  styleUrls: ['./channel-playlists-view-selector.component.css'],
})
export class ChannelPlaylistsViewSelectorComponent
  implements OnInit, OnChanges
{
  @Input() channel!: UserChannel;
  @Input() view!: ViewTarget;
  @Input() sectionId: string | null = null;

  viewSelections: ViewSelection[] = [];
  selectedView!: ViewSelection;

  constructor(private router: Router) {}

  ngOnInit(): void {}

  ngOnChanges(changes: SimpleChanges) {
    this.refreshViewSelections();
    this.refreshSelectedView(this.view, this.sectionId);
  }

  private refreshViewSelections() {
    const createdPlaylistsSelection: ViewSelection = {
      viewTarget: ViewTarget.Created,
      displayName: 'Created playlists',
    };

    const sectionSelections = this.channel.sections
      .filter((section): section is MultiplePlaylistsSection => {
        return section.type == ChannelSectionType.MultiplePLaylists;
      })
      .map((section) => {
        let title = section.content.title.trim();
        if (title == '') {
          title = 'Multiple playlists';
        }

        const selection: ViewSelection = {
          viewTarget: ViewTarget.Section,
          displayName: title,
          sectionId: section.id!,
        };
        return selection;
      });

    this.viewSelections = [];

    if (this.channel.sections.length > 1) {
      const overviewSelection: ViewSelection = {
        viewTarget: ViewTarget.Overview,
        displayName: 'All playlists',
      };

      this.viewSelections.push(overviewSelection);
    }

    this.viewSelections.push(createdPlaylistsSelection);

    this.viewSelections.push(...sectionSelections);
  }

  refreshSelectedView(view: ViewTarget, sectionId: string | null) {
    if (view == ViewTarget.Section) {
      this.selectedView = this.viewSelections.find(
        (x) => x.sectionId == sectionId
      )!;
    } else {
      this.selectedView = this.viewSelections.find(
        (x) => x.viewTarget == view
      )!;
    }
  }

  selectView(selection: ViewSelection) {
    let view;

    switch (selection.viewTarget) {
      case ViewTarget.Overview:
        view = 'overview';
        break;
      case ViewTarget.Created:
        view = 'created';
        break;
      case ViewTarget.Section:
        view = selection.sectionId!;
        break;
    }

    this.router.navigate([], {
      queryParams: {
        view,
      },
    });
  }
}

export interface ViewSelection {
  viewTarget: ViewTarget;
  displayName: string;
  sectionId?: string;
}
