namespace Library.API.Application.Configurations {
    public class PlaylistQueryConfiguration {

        /// <summary>
        /// When trying to find the first available video in a playlist
        /// this is the maximum number of non-avaialble videos that can be placed at the beginning
        /// to cause the search for the first available video to fail.
        /// <para>
        /// A similar number for Youtube is 10.
        /// You can try to create a non-private playlist and add 10 private videos in front of a non-private video.
        /// Share the playlist to other user. The other user will fail to navigate to the
        /// first non-private video by clicking the playlist in his library page.</para>
        /// </summary>
        public int NonAvailableVideosTolerance { get; set; } = 10;

    }
}
