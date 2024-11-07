using System.ComponentModel;

namespace IntelligentPlant.DataCore.Client.Model {
    /// <summary>
    /// Describes the features supported by a data source driver.
    /// </summary>
    [Flags]
    public enum DataSourceDriverFeatures {

        /// <summary>
        /// The driver does not support any expected features.
        /// </summary>
        [Description("The driver does not support any expected features.")]
        None = 0,

        /// <summary>
        /// The driver supports tag searches.
        /// </summary>
        [Description("The driver supports tag searches.")]
        TagSearch = 1,

        /// <summary>
        /// The driver supports tag value queries.
        /// </summary>
        [Description("The driver supports tag value queries.")]
        ReadTagValues = 2,

        /// <summary>
        /// The driver supports writing tag values back to the data source.
        /// </summary>
        [Description("The driver supports writing tag values back to the data source.")]
        WriteTagValues = 4,

        /// <summary>
        /// The driver supports creation of new tags in the data source.
        /// </summary>
        [Description("The driver supports creation of new tags in the data source.")]
        CreateTags = 8,

        /// <summary>
        /// The driver supports updating existing tags in the data source.
        /// </summary>
        [Description("The driver supports updating existing tags in the data source.")]
        UpdateTags = 16,

        /// <summary>
        /// The driver allows data source tags to be deleted.
        /// </summary>
        [Description("The driver allows data source tags to be deleted.")]
        DeleteTags = 32,

        /// <summary>
        /// The driver supports push notifications for tag values.
        /// </summary>
        [Description("The driver supports push notifications for tag values.")]
        ValuePushNotifications = 64,

        /// <summary>
        /// The driver supports asset model browsing.
        /// </summary>
        [Description("The driver supports asset model browsing.")]
        AssetModelBrowsing = 128,

        /// <summary>
        /// The driver supports management of tag value annotations.
        /// </summary>
        [Description("The driver supports management of tag value annotations.")]
        AnnotationManagement = 256,

        /// <summary>
        /// 
        /// </summary>
        [Description("")]
        ReadEvent = 512,

        /// <summary>
        /// 
        /// </summary>
        [Description("")]
        WriteEvent = 1024,

        /// <summary>
        /// 
        /// </summary>
        [Description("")]
        AlarmAnalysisQuery = 2048,

        /// <summary>
        /// The driver supports push notifications for events.
        /// </summary>
        [Description("The driver supports push notifications for events.")]
        EventPushNotifications = 4096,

        /// <summary>
        /// The driver supports scripting.
        /// </summary>
        [Description("The driver supports scripting.")]
        Scripting = 8192
    }
}
