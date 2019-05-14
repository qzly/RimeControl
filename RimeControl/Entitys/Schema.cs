using System.ComponentModel;

namespace RimeControl.Entitys
{
    //输入法方案
    public class Schema : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 使用中
        /// </summary>
        public bool IsUsing { get; set; }

        private bool _isSelect;

        /// <summary>
        /// 勾选
        /// </summary>
        public bool IsSelect
        {
            get { return _isSelect; }
            set {
                _isSelect = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("isSelect"));
                }
            }
        }


        /// <summary>
        /// 预设方案
        /// </summary>
        public bool IsSys { get; set; }
        /// <summary>
        /// schema id
        /// </summary>
        public string SchemaId { get; set; }
        /// <summary>
        /// schema 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// schema 版本信息
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// schema 作者信息
        /// </summary>
        public string Author { get; set; }
        /// <summary>
        /// schema描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 依赖的 dependencies  
        /// 依赖的Schema Id数组
        /// </summary>
        public string Dependencies { get; set; }
        /// <summary>
        /// 是否在Roaming\Rime目录
        /// </summary>
        public bool InRoaming { get; set; }

        /// <summary>
        /// 自动纠错功能
        /// 感谢 github Youxikong
        /// </summary>
        public bool EnableCorrection { get; set; }
        /// <summary>
        /// 记录读取时是否已经启用 自动纠错功能
        /// </summary>

        public bool OldEnableCorrection { get; set; }
    }
}
