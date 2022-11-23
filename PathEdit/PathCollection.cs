using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PathEdit
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPathCollection
    {
        string EnvironmentVariable { get; }
        string PathSeparator { get; }

        string FullPath { get; }

        int Count { get; }
        bool IsDirty { get; }
        string[] ToArray();

        void Refresh();
        string GetPath(int index);
        void SetPath(int index, string path);
        int AddPath(string path);
        void InsertPath(int index, string path);
        void RemovePath(int index);
        void ClearPaths();
    }

    /// <summary>
    /// 
    /// </summary>
    public class PathCollection : IPathCollection
    {
        public const string REG_KEY_PATH = @"HKLM\SYSTEM\CurrentControlSet\Control\Session Manager\Environment";

        private string _EnvironmentVariable;
        private string _PathSeparator;
        private List<string> _Paths;
        private bool _IsDirty;

        public PathCollection(string environmentVariable, string pathSeparator)
        {
            _EnvironmentVariable = environmentVariable;
            _PathSeparator = pathSeparator;
            Refresh();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            Create();
            _Paths.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        private void Create()
        {
            if (_Paths == null)
                _Paths = new List<string>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private bool IsValidIndex(int index)
        {
            return index >= 0 && index < Count;
        }

        #region IPathCollection Members

        /// <summary>
        /// 
        /// </summary>
        public string EnvironmentVariable
        {
            get { return _EnvironmentVariable; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string PathSeparator
        {
            get { return _PathSeparator; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string FullPath
        {
            get { return string.Join(PathSeparator, _Paths.ToArray()); }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsDirty
        {
            get { return _IsDirty; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Count
        {
            get { return _Paths == null ? 0 : _Paths.Count; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string[] ToArray()
        {
            Create();
            return _Paths.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Refresh()
        {
            Clear();

            _Paths.AddRange(Environment.GetEnvironmentVariable(_EnvironmentVariable).Split(_PathSeparator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries));

            _IsDirty = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string GetPath(int index)
        {
            return IsValidIndex(index) ? _Paths[index] : null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="path"></param>
        public void SetPath(int index, string path)
        {
            if (IsValidIndex(index))
                _Paths[index] = path;
            else
                AddPath(path);

            _IsDirty = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public int AddPath(string path)
        {
            Create();
            _Paths.Add(path);

            _IsDirty = true;

            return _Paths.Count - 1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public void InsertPath(int index, string path)
        {
            Create();

            _Paths.Insert(index, path);

            _IsDirty = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public void RemovePath(int index)
        {
            Create();
            _Paths.RemoveAt(index);

            _IsDirty = true;
        }

        /// <summary>
        /// 
        /// </summary>
        public void ClearPaths()
        {
            Clear();

            _IsDirty = true;
        }

        #endregion
    }
}
