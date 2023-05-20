using LogicSimulator.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace LogicSimulator.Models
{
    public class FileHandler
    {
        readonly static string dir = "../../../../storage/";
        readonly List<Project> projects = new();

        public FileHandler()
        {
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            foreach (var fullname in Directory.EnumerateFiles(dir))
            {
                string name = fullname.Split("/")[^1];
                if (name.StartsWith("proj_")) LoadProject(name);
            }
        }



        public static string GetProjectFileName()
        {
            for (int i = 1; ; i++)
            {
                string name = "proj_" + i + ".json";
                if (!File.Exists(dir + name)) return name;
            }
        }
        public static string GetSchemeFileName()
        {
            for (int i = 1; ; i++)
            {
                string name = "scheme_" + i + ".yaml";
                if (!File.Exists(dir + name)) return name;
            }
        }



        public Project CreateProject()
        {
            var proj = new Project();
            projects.Add(proj);
            return proj;
        }
        private Project? LoadProject(string fileName)
        {
            try
            {
                var obj = Utils.Json2obj(File.ReadAllText(dir + fileName)) ?? throw new DataException("Неверная структура JSON-файла проекта!");
                var proj = new Project(fileName, obj);
                projects.Add(proj);
                return proj;
            }
            catch (Exception e) { Log.Write("Не удалось загрузить проект:" + Environment.NewLine + e); }
            return null;
        }
        public static Scheme? LoadScheme(Project parent, string fileName)
        {
            try
            {
                var obj = Utils.Yaml2obj(File.ReadAllText(dir + fileName)) ?? throw new DataException("Неверная структура схемы YAML-файла.");
                var scheme = new Scheme(parent, fileName, obj);
                return scheme;
            }
            catch (Exception e) { Log.Write("Не удалось загрузить схему:" + Environment.NewLine + e); }
            return null;
        }



        public static void SaveProject(Project proj)
        {
            var data = Utils.Obj2json(proj.Export());
            File.WriteAllText(dir + proj.FileName, data);
        }
        public static void SaveScheme(Scheme scheme)
        {
            var data = Utils.Obj2yaml(scheme.Export());
            File.WriteAllText(dir + scheme.FileName, data);
        }

        public Project[] GetSortedProjects()
        {
            projects.Sort();
            return projects.ToArray();
        }
    }
}
