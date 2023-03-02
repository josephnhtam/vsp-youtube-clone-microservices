using AutoMapper;
using SharedKernel.Exceptions;
using Users.API.Application.DtoModels;
using Users.Domain.Models;

namespace Users.API.Application.AutoMapperProfiles {
    public class ChannelSectionsAutoMapperProfile : Profile {

        public ChannelSectionsAutoMapperProfile () {
            CreateMap<VideosSection, ChannelSectionDto>()
                .ConvertUsing(section => new ChannelSectionDto {
                    Type = ChannelSectionType.Videos,
                    Id = section.Id,
                    Content = new SectionContent()
                });

            CreateMap<CreatedPlaylistsSection, ChannelSectionDto>()
                .ConvertUsing(section => new ChannelSectionDto {
                    Type = ChannelSectionType.CreatedPlaylists,
                    Id = section.Id,
                    Content = new SectionContent()
                });

            CreateMap<SinglePlaylistSection, ChannelSectionDto>()
                .ConvertUsing(section => new ChannelSectionDto {
                    Type = ChannelSectionType.SinglePlaylist,
                    Id = section.Id,
                    Content = new SectionContent {
                        PlaylistId = section.Items != null ? section.Items.FirstOrDefault() : null
                    }
                });

            CreateMap<MultiplePlaylistsSection, ChannelSectionDto>()
                .ConvertUsing(section => new ChannelSectionDto {
                    Type = ChannelSectionType.MultiplePLaylists,
                    Id = section.Id,
                    Content = new SectionContent {
                        Title = section.Title,
                        PlaylistIds = section.Items
                    }
                });

            // =======================================================

            CreateMap<ChannelSectionDto, ChannelSection>()
                .ConvertUsing((section, ctx) => {
                    Guid id = section.Id ?? Guid.NewGuid();

                    switch (section.Type) {
                        case ChannelSectionType.Videos: {
                                return new VideosSection(id);
                            }
                        case ChannelSectionType.CreatedPlaylists: {
                                return new CreatedPlaylistsSection(id);
                            }
                        case ChannelSectionType.SinglePlaylist: {
                                if (section.Content != null &&
                                    section.Content.PlaylistId.HasValue) {
                                    return new SinglePlaylistSection(id, section.Content.PlaylistId.Value);
                                }

                                break;
                            }
                        case ChannelSectionType.MultiplePLaylists: {
                                if (section.Content != null) {
                                    return new MultiplePlaylistsSection(
                                        id,
                                        section.Content.Title ?? string.Empty,
                                        section.Content.PlaylistIds ?? new List<Guid>());
                                }

                                break;
                            }
                    }

                    throw new AppException(
                        "Failed to recognize channel section",
                        null,
                        StatusCodes.Status400BadRequest);
                });
        }

    }
}
