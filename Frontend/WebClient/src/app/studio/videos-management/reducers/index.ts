import {VideoProcessingStatus, VideoStatus, VideoThumbnailStatus,} from './../../models';
import {createReducer, on} from '@ngrx/store';
import {VideosManagementAction, VideosManagementApiAction, VideosManagementHubAction,} from '../actions';
import {VideoClient} from '../models';

export interface State {
  videoClients: VideoClient[];
  totalVideosCount: number;
  isFetchingVideos: boolean;
  lastFetchVideoClientIds: string[];
}

const initialState: State = {
  videoClients: [],
  totalVideosCount: 0,
  isFetchingVideos: false,
  lastFetchVideoClientIds: [],
};

export const reducer = createReducer(
  initialState,

  on(
    VideosManagementApiAction.failedToUnregisterVideo,
    (state, { videoId, error }) => {
      return {
        ...state,
        videoClients: state.videoClients.map((vc) => {
          if (vc.video?.id == videoId) {
            return {
              ...vc,
              pending: false,
              error: error,
            };
          } else {
            return vc;
          }
        }),
      };
    }
  ),

  on(VideosManagementApiAction.videoUnregistered, (state, { videoId }) => {
    return {
      ...state,
      videoClients: state.videoClients.filter((vc) => vc.video?.id != videoId),
    };
  }),

  on(VideosManagementAction.unregisterVideo, (state, { videoId }) => {
    return {
      ...state,
      videoClients: state.videoClients.map((vc) => {
        if (vc.video?.id == videoId) {
          return {
            ...vc,
            pending: true,
          };
        } else {
          return vc;
        }
      }),
    };
  }),

  on(
    VideosManagementApiAction.failedToUpdateVideoInfo,
    (state, { videoId }) => {
      return {
        ...state,
        videoClients: state.videoClients.map((vc) => {
          if (vc.video?.id == videoId) {
            return {
              ...vc,
              pending: true,
            };
          } else {
            return vc;
          }
        }),
      };
    }
  ),

  on(VideosManagementAction.setVideoInfo, (state, { request }) => {
    return {
      ...state,
      videoClients: state.videoClients.map((vc) => {
        if (vc.video?.id == request.videoId) {
          return {
            ...vc,
            pending: true,
          };
        } else {
          return vc;
        }
      }),
    };
  }),

  on(VideosManagementAction.clearLastFetchVideoClients, (state) => {
    return {
      ...state,
      lastFetchVideoClientIds: [],
    };
  }),

  on(VideosManagementApiAction.failedToObtainVideos, (state) => {
    return {
      ...state,
      isFetchingVideos: false,
    };
  }),

  on(VideosManagementApiAction.videosObtained, (state, { response }) => {
    let videoClients: VideoClient[] = state.videoClients.map((vc) => {
      const video = response.videos.find((v) => v.id === vc.video?.id);
      if (video) {
        return {
          ...vc,
          video,
        };
      } else {
        return vc;
      }
    });

    const newVideoClients = response.videos
      .filter((v) => !state.videoClients.some((vc) => vc.video?.id === v.id))
      .map((video) => {
        const newVideoClient: VideoClient = {
          video,
          videoUploadToken: null,
          pending: false,
          processError: null,
          error: null,
        };
        return newVideoClient;
      });

    videoClients = [...videoClients, ...newVideoClients];

    return {
      ...state,
      totalVideosCount: response.totalCount,
      isFetchingVideos: false,
      lastFetchVideoClientIds: response.videos.map((x) => x.id),
      videoClients,
    };
  }),

  on(VideosManagementAction.getVideos, (state) => {
    return {
      ...state,
      isFetchingVideos: true,
    };
  }),

  on(
    VideosManagementApiAction.videoObtained,
    VideosManagementApiAction.videoInfoUpdated,
    (state, { type, video }) => {
      const update = state.videoClients.some((vc) => vc.video?.id === video.id);

      if (update) {
        return {
          ...state,
          videoClients: state.videoClients.map((videoClient) => {
            if (videoClient.video?.id === video.id) {
              const pending =
                type === VideosManagementApiAction.videoInfoUpdated.type
                  ? false
                  : videoClient.pending;

              return {
                ...videoClient,
                video,
                pending,
              };
            } else {
              return videoClient;
            }
          }),
        };
      } else {
        const newVideoClient: VideoClient = {
          video,
          pending: false,
          videoUploadToken: null,
          error: null,
          processError: null,
        };

        let videoClients = [...state.videoClients, newVideoClient];

        return {
          ...state,
          videoClients: videoClients,
        };
      }
    }
  ),

  on(
    VideosManagementHubAction.videoThumbnailsAdded,
    (state, { videoId, thumbnails }) => {
      return {
        ...state,
        videoClients: state.videoClients.map((videoClient) => {
          if (videoClient?.video?.id === videoId) {
            return {
              ...videoClient,
              video: {
                ...videoClient.video,
                thumbnailStatus: VideoThumbnailStatus.Processed,
                thumbnails,
              },
              processingStatus: Math.max(
                videoClient.video.processingStatus,
                VideoProcessingStatus.VideoBeingProcessed
              ),
            };
          } else {
            return videoClient;
          }
        }),
      };
    }
  ),

  on(
    VideosManagementHubAction.processedVideoAdded,
    (state, { videoId, video }) => {
      return {
        ...state,
        videoClients: state.videoClients.map((videoClient) => {
          if (videoClient?.video?.id === videoId) {
            return {
              ...videoClient,
              video: {
                ...videoClient.video,
                videos: videoClient.video.videos
                  .filter((x) => x.videoFileId != video.videoFileId)
                  .concat(video),
                processingStatus: Math.max(
                  videoClient.video.processingStatus,
                  VideoProcessingStatus.VideoBeingProcessed
                ),
              },
            };
          } else {
            return videoClient;
          }
        }),
      };
    }
  ),

  on(VideosManagementHubAction.videoProcessingComplete, (state, { video }) => {
    return {
      ...state,
      videoClients: state.videoClients.map((videoClient) => {
        if (videoClient?.video?.id === video.id) {
          return {
            ...videoClient,
            video,
          };
        } else {
          return videoClient;
        }
      }),
    };
  }),

  on(VideosManagementHubAction.videoProcessingFailed, (state, { videoId }) => {
    return {
      ...state,
      videoClients: state.videoClients.map((videoClient) => {
        if (videoClient?.video?.id === videoId) {
          return {
            ...videoClient,
            video: {
              ...videoClient.video,
              processingStatus: VideoProcessingStatus.VideoProcessingFailed,
            },
          };
        } else {
          return videoClient;
        }
      }),
    };
  }),

  on(VideosManagementHubAction.videoBeingProcessed, (state, { videoId }) => {
    return {
      ...state,
      videoClients: state.videoClients.map((videoClient) => {
        if (videoClient?.video?.id === videoId) {
          return {
            ...videoClient,
            video: {
              ...videoClient.video,
              status: Math.max(
                videoClient.video.status,
                VideoStatus.Registered
              ),
              processingStatus: Math.max(
                videoClient.video.processingStatus,
                VideoProcessingStatus.VideoBeingProcessed
              ),
            },
          };
        } else {
          return videoClient;
        }
      }),
    };
  }),

  on(
    VideosManagementHubAction.videoUploaded,
    (state, { videoId, originalFileName, videoFileUrl }) => {
      return {
        ...state,
        videoClients: state.videoClients.map((videoClient) => {
          if (videoClient?.video?.id === videoId) {
            return {
              ...videoClient,
              video: {
                ...videoClient.video,
                originalVideoFileName: originalFileName,
                originalVideoUrl: videoFileUrl,
                processingStatus: Math.max(
                  videoClient.video.processingStatus,
                  VideoProcessingStatus.VideoUploaded
                ),
              },
            };
          } else {
            return videoClient;
          }
        }),
      };
    }
  ),

  on(
    VideosManagementApiAction.failedToUploadVideo,
    (state, { videoId, error }) => {
      return {
        ...state,
        videoClients: state.videoClients.map((videoClient) => {
          if (videoClient.video?.id === videoId) {
            return {
              ...videoClient,
              processError: error,
            };
          } else {
            return videoClient;
          }
        }),
      };
    }
  ),

  on(VideosManagementAction.createVideo, (state, { contextId }) => {
    const videoClient: VideoClient = {
      contextId: contextId,
      pending: true,
      videoUploadToken: null,
      error: null,
      processError: null,
    };

    let videoClients = state.videoClients.filter(
      (x) => !contextId || x.contextId !== contextId
    );

    videoClients = [...videoClients, videoClient];

    return {
      ...state,
      videoClients,
    };
  }),

  on(VideosManagementApiAction.videoCreated, (state, { video, contextId }) => {
    return {
      ...state,
      videoClients: state.videoClients.map((videoClient) => {
        if (contextId && videoClient.contextId === contextId) {
          return {
            ...videoClient,
            pending: false,
            video,
          };
        } else {
          return videoClient;
        }
      }),
    };
  }),

  on(
    VideosManagementApiAction.failedToCreateVideo,
    (state, { error, contextId }) => {
      return {
        ...state,
        videoClients: state.videoClients.map((videoClient) => {
          if (contextId && videoClient.contextId === contextId) {
            return {
              ...videoClient,
              pending: false,
              processError: error,
            };
          } else {
            return videoClient;
          }
        }),
      };
    }
  ),

  on(VideosManagementAction.getVideoUploadToken, (state, { contextId }) => {
    return {
      ...state,
      videoClients: state.videoClients.map((videoClient) => {
        if (contextId && videoClient.contextId === contextId) {
          return {
            ...videoClient,
            pending: true,
          };
        } else {
          return videoClient;
        }
      }),
    };
  }),

  on(
    VideosManagementApiAction.videoUploadTokenObtained,
    (state, { videoId, videoUploadToken }) => {
      return {
        ...state,
        videoClients: state.videoClients.map((videoClient) => {
          if (videoId === videoClient.video?.id) {
            return {
              ...videoClient,
              pending: false,
              videoUploadToken,
            };
          } else {
            return videoClient;
          }
        }),
      };
    }
  ),

  on(
    VideosManagementApiAction.failedToObtainVideoUploadToken,
    (state, { error, videoId }) => {
      return {
        ...state,
        videoClients: state.videoClients.map((videoClient) => {
          if (videoId === videoClient.video?.id) {
            return {
              ...videoClient,
              pending: false,
              videoUploadToken: null,
              processError: error,
            };
          } else {
            return videoClient;
          }
        }),
      };
    }
  )
);
