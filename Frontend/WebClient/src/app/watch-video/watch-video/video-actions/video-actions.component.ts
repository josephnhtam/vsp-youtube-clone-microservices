import {WatchVideoComponent} from '../watch-video.component';
import {AuthService} from '../../../auth/services/auth.service';
import {VideoMetadata, VoteType} from '../../../core/models/library';
import {VideoActionService} from './video-actions.service';
import {Component, Input, OnDestroy, OnInit} from '@angular/core';
import {Video} from '../../../core/models/video';
import {UserProfileService} from 'src/app/core/services/user-profile.service';
import {Subscription, take, tap} from 'rxjs';

@Component({
  selector: 'app-video-actions',
  templateUrl: './video-actions.component.html',
  styleUrls: ['./video-actions.component.css'],
})
export class VideoActionComponent implements OnInit, OnDestroy {
  @Input()
  watchComp!: WatchVideoComponent;

  metadata?: VideoMetadata;
  videoSub?: Subscription;

  private video!: Video;

  constructor(
    private actions: VideoActionService,
    private authService: AuthService,
    private userProfileService: UserProfileService
  ) {}

  ngOnDestroy(): void {
    this.videoSub?.unsubscribe();
  }

  ngOnInit(): void {
    this.videoSub = this.watchComp!.video$.subscribe((video) => {
      this.video = video;

      this.metadata = undefined;
      this.actions.getVideoMetadata(this.video.id).subscribe({
        next: (metadata) => {
          this.metadata = metadata;
        },
      });
    });
  }

  get isLoaded() {
    return !!this.metadata;
  }

  get likesCount() {
    return this.metadata?.likesCount ?? 0;
  }

  get dislikesCount() {
    return this.metadata?.likesCount ?? 0;
  }

  get isLiked() {
    return this.metadata?.userVote == VoteType.Like;
  }

  get isDisliked() {
    return this.metadata?.userVote == VoteType.Dislike;
  }

  get isAuthenticated$() {
    return this.authService.isAuthenticated$;
  }

  get isUserReady$() {
    return this.userProfileService.userReady$;
  }

  voteLike() {
    this.voteVideo(VoteType.Like);
  }

  voteDislike() {
    this.voteVideo(VoteType.Dislike);
  }

  voteVideo(voteType: VoteType) {
    if (!this.metadata) return;

    if (voteType == this.metadata.userVote && voteType != VoteType.None) {
      voteType = VoteType.None;
    } else if (voteType != VoteType.None) {
      if (voteType == VoteType.Like) {
        this.metadata.likesCount++;
      } else {
        this.metadata.dislikesCount++;
      }
    }

    if (voteType != this.metadata.userVote) {
      if (this.metadata.userVote == VoteType.Like) {
        this.metadata.likesCount = Math.max(0, this.metadata.likesCount - 1);
      } else if (this.metadata.userVote == VoteType.Dislike) {
        this.metadata.dislikesCount = Math.max(
          0,
          this.metadata.dislikesCount - 1
        );
      }
    }

    this.metadata.userVote = voteType;

    this.actions.voteVideo(this.video.id, voteType).subscribe();
  }

  addToPlaylist() {
    this.watchComp.playlistItemId$
      .pipe(
        take(1),
        tap((itemId) => {
          this.actions.openAddToPlaylistDialog(this.video.id, itemId);
        })
      )
      .subscribe();
  }
}
