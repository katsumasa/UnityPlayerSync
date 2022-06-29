//
// Programed by Katsumasa Kimura
//
namespace UTJ.UnityPlayerSync
{    
    /// <summary>
    /// GUID
    /// </summary>
    public static class UnityPlayerSyncGuid
    {
        // Editor -> Player�p GUID
        public static readonly System.Guid kMsgSendEditorToPlayer = new System.Guid("6f9b9bd0348d42099408800612610f01");
        // Player -> Editor�p GUID
        public static readonly System.Guid kMsgSendPlayerToEditor = new System.Guid("a673119d62c6482a967fce74b4ec12d8");
    }
  
    
    /// <summary>
    /// ���b�Z�[�W���ʎq
    /// </summary>
    public enum MessageID
    {
        SyncScene,      // Scene�̓���
        SyncTransform,  // Transform�̓���
        SyncComponent,  // Component�̓���
    };
}
