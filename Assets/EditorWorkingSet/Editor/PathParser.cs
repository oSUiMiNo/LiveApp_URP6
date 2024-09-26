using UnityEngine;
using System.Collections;
namespace WorkingSet
{
    public class PathParser
    {
        string path;
        string file_name;
        string file_ext;

        //full_path must be a file path
        public PathParser(string full_path)
        {
            Paser(full_path);
        }

        void Paser(string full_path)
        {
            path = GetPath(full_path);
            string full_name = GetFullFileName(full_path);
            SeprateFileName(full_name, out file_name, out file_ext);
        }

        public string FullPath
        {
            get
            {
                return CombinePaths(path, FullFileName);
            }
            set { Paser(value); }
        }
        // 返回 末尾不带 "\" 的目录名称
        public string Path
        {
            get { return path; }
            set
            {
                path = RegularPath(value);
                if (!path.EndsWith("" + System.IO.Path.DirectorySeparatorChar))
                {
                    path += System.IO.Path.DirectorySeparatorChar;
                }
            }
        }
        /// <summary>
        /// 返回文件名字 包括扩展名
        /// </summary>
        /// <param name="full_path"></param>
        /// <returns></returns>
        public string FileName
        {
            get { return file_name; }
            set { file_name = value; }
        }
        /// <summary>
        /// 返回文件扩展名
        /// </summary>
        /// <param name="full_path"></param>
        /// <returns></returns>
        public string FileExtension
        {
            get { return file_ext; }
            set { file_ext = value; }
        }
        /// <summary>
        /// 文件名+扩展名
        /// </summary>
        public string FullFileName
        {
            get
            {
                if (file_ext == "") return file_name;
                else return file_name + "." + file_ext;
            }
            set
            {
                SeprateFileName(value, out file_name, out file_ext);
            }
        }

        //full_path must be a file path
        public static PathParser Parse(string full_path)
        {
            return new PathParser(full_path);
        }

        /// <summary>
        /// 返回目录名称
        /// </summary>
        /// <param name="full_path"></param>
        /// <returns></returns>
        public static string GetPath(string full_path, bool end_with_sperator = true)
        {
            full_path = RegularPath(full_path);
            int find = -1;
            for (int i = full_path.Length - 1; i >= 0; i--)
            {
                if (full_path[i] == '/' || full_path[i] == '\\' || full_path[i] == System.IO.Path.DirectorySeparatorChar)
                {
                    find = i;
                    break;
                }
            }
            string path = full_path;
            if (find != -1) path = full_path.Substring(0, find);

            if (end_with_sperator && !path.EndsWith("" + System.IO.Path.DirectorySeparatorChar))
            {
                path += System.IO.Path.DirectorySeparatorChar;
            }
            return path;
        }

        static string RegularPath(string full_path)
        {
            full_path = full_path.Replace('/', System.IO.Path.DirectorySeparatorChar);
            full_path = full_path.Replace('\\', System.IO.Path.DirectorySeparatorChar);
            return full_path;
        }


        /// <summary>
        /// 返回文件名字 包括扩展名
        /// </summary>
        /// <param name="full_path"></param>
        /// <returns></returns>
        public static string GetFullFileName(string full_path)
        {
            full_path = RegularPath(full_path);
            int find = -1;
            for (int i = full_path.Length - 1; i >= 0; i--)
            {
                if (full_path[i] == System.IO.Path.DirectorySeparatorChar)
                {
                    find = i;
                    break;
                }
            }
            if (find == -1) return full_path;
            return full_path.Substring(find + 1, full_path.Length - find - 1);
        }

        /// <summary>
        /// 返回文件名字 包括扩展名
        /// </summary>
        /// <param name="full_path"></param>
        /// <returns></returns>
        public static void SeprateFileName(string full_file_name, out string file_name, out string file_extention)
        {
            int find = -1;
            for (int i = full_file_name.Length - 1; i >= 0; i--)
            {
                if (full_file_name[i] == '.')
                {
                    find = i;
                    break;
                }
            }
            if (find == -1)
            {
                file_name = full_file_name;
                file_extention = "";
            }
            else
            {
                file_name = full_file_name.Substring(0, find);
                file_extention = full_file_name.Substring(find + 1, full_file_name.Length - find - 1);
            }
        }

        public static string CombinePaths(params string[] paths)
        {

            string full = "";
            for (int i = 0; i < paths.Length; i++)
            {
                paths[i] = RegularPath(paths[i]);

                full += paths[i];
                if (i != paths.Length - 1)
                {
                    if (paths[i].EndsWith("" + System.IO.Path.DirectorySeparatorChar))
                    {
                    }
                    else
                    {
                        full += System.IO.Path.DirectorySeparatorChar;
                    }
                }

            }
            return full;
        }

        public static bool PathEqual(string path1, string path2)
        {
            path1 = RegularPath(path1);
            path2 = RegularPath(path2);

            path1 = path1.TrimEnd(System.IO.Path.DirectorySeparatorChar);
            path2 = path2.TrimEnd(System.IO.Path.DirectorySeparatorChar);

            return path1 == path2;
        }
    }
}