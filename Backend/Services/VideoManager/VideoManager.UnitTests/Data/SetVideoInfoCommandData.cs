using Moq;
using System.Collections;
using VideoManager.API.Commands;
using VideoManager.Domain.Models;
using VideoManager.Domain.Rules.Videos;

namespace VideoManager.UnitTests.Data {
    public class SetVideoInfoCommandData : IEnumerable<object[]> {

        public IEnumerator<object[]> GetEnumerator () {
            string validTile = Utilities.GenerateString(TitleLengthRule.MaxLength);
            string validDescription = Utilities.GenerateString(DescriptionLengthRule.MaxLength);
            string validTags = Utilities.GenerateString(TagsLengthRule.MaxLength);

            string tooLongTitle = Utilities.GenerateString(TitleLengthRule.MaxLength + 1);
            string tooLongDescription = Utilities.GenerateString(DescriptionLengthRule.MaxLength + 1);
            string toLongTags = Utilities.GenerateString(TagsLengthRule.MaxLength + 1);

            yield return new object[] { false, new SetVideoInfoCommand {
                    SetVideoBasicInfo = new () {
                        Title = string.Empty,
                        Description = string.Empty,
                        Tags = string.Empty
                    },
                    SetVideoVisibilityInfo = new () {
                        Visibility = It.IsAny<VideoVisibility>()
                    }
                }
            };

            yield return new object[] { true, new SetVideoInfoCommand {
                    SetVideoBasicInfo = new () {
                        Title = validTile,
                        Description = string.Empty,
                        Tags = string.Empty
                    },
                    SetVideoVisibilityInfo = new () {
                        Visibility = It.IsAny<VideoVisibility>()
                    }
                }
            };

            yield return new object[] { true, new SetVideoInfoCommand {
                    SetVideoBasicInfo = new () {
                        Title = validTile,
                        Description = validDescription,
                        Tags = validTags
                    },
                    SetVideoVisibilityInfo = new () {
                        Visibility = It.IsAny<VideoVisibility>()
                    }
                }
            };

            yield return new object[] { false, new SetVideoInfoCommand {
                    SetVideoBasicInfo = new () {
                        Title = tooLongTitle,
                        Description = validDescription,
                        Tags = validTags
                    },
                    SetVideoVisibilityInfo = new () {
                        Visibility = It.IsAny<VideoVisibility>()
                    }
                }
            };

            yield return new object[] { false, new SetVideoInfoCommand {
                    SetVideoBasicInfo = new () {
                        Title = validTile,
                        Description = tooLongDescription,
                        Tags = validTags
                    },
                    SetVideoVisibilityInfo = new () {
                        Visibility = It.IsAny<VideoVisibility>()
                    }
                }
            };

            yield return new object[] { false, new SetVideoInfoCommand {
                    SetVideoBasicInfo = new () {
                        Title = validTile,
                        Description = validDescription,
                        Tags = toLongTags
                    },
                    SetVideoVisibilityInfo = new () {
                        Visibility = It.IsAny<VideoVisibility>()
                    }
                }
            };

            yield return new object[] { false, new SetVideoInfoCommand {
                    SetVideoBasicInfo = new () {
                        Title = validTile,
                        Description = validDescription,
                        Tags = validTags
                    },
                    SetVideoVisibilityInfo = new () {
                        Visibility = (VideoVisibility)(-1)
                    }
                }
            };
        }

        IEnumerator IEnumerable.GetEnumerator () => GetEnumerator();
    }
}
