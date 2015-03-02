using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace System.Common.References
{
  /// <summary>
  /// Represents a visual studio project within a solution file.
  /// </summary>
  [DebuggerDisplay("{Name} : {Path} : {IsSolutionFolder}")]
  public sealed class VSProject
  {
    const string SolutionFolder = "{2150E333-8FDC-42A3-9474-1A3956D46DE8}";
    static readonly char[] tokens = "()\",=\n\r".ToCharArray();

    /// <summary>Gets a Guid representing the type for this project.</summary>
    public string TypeGuid { get; private set; }

    /// <summary>Gets a the name of this project.</summary>
    public string Name { get; private set; }

    /// <summary>Gets the path relative to the solution.</summary>
    public string Path { get; private set; }

    /// <summary>Gets the Guid representing the project's unique ID.</summary>
    public string Guid { get; private set; }

    /// <summary>Gets a value indicating if this project represents a solution folder</summary>
    public bool IsSolutionFolder { get; private set; }

    /// <summary>
    /// Create a new project using the text between Project and EndProject within a solution file.
    /// </summary>
    /// <param name="project">The text between Project and EndProject within a solution file.</param>
    public VSProject(string project)
    {
      var parts = project
        .Split(tokens, StringSplitOptions.RemoveEmptyEntries)
        .Select(t => t.Trim())
        .Where(t => !string.IsNullOrEmpty(t))
        .ToArray();

      var i = 1;
      TypeGuid = parts[i++];
      Name = parts[i++];
      Path = parts[i++];
      Guid = parts[i++];
      IsSolutionFolder = string.Equals(TypeGuid, SolutionFolder);
    }
  }

  /// <summary>
  /// Represents a visual studio solution file.
  /// </summary>
  public sealed class VSSolution
  {
    const string BeginToken = "Project(\"";
    const string EndToken = "EndProject";

    private List<VSProject> projects = new List<VSProject>();

    /// <summary>
    /// All of the projects within this solution file.
    /// </summary>
    public IEnumerable<VSProject> Projects
    {
      get
      {
        foreach (var project in projects)
          yield return project;
      }
    }

    private VSSolution() { }

    private void ParseData(string data)
    {
      int bIdx = data.IndexOf(BeginToken), eIdx;
      while (bIdx > -1)
      {
        eIdx = data.IndexOf(EndToken, bIdx + 1);
        string project = data.Substring(bIdx, (eIdx - bIdx) + EndToken.Length);
        projects.Add(new VSProject(project));
        bIdx = data.IndexOf(BeginToken, eIdx);
      }
    }

    /// <summary>
    /// Parses a solution file.
    /// </summary>
    /// <param name="filepath">The path to the solution file.</param>
    /// <returns>A VSSolution object containing the projects.</returns>
    public static VSSolution Parse(string filepath)
    {
      using (var stream = File.OpenRead(filepath))
        return Parse(stream);
    }

    /// <summary>
    /// Parses a solution file.
    /// </summary>
    /// <param name="stream">A stream containing the solution file bytes.</param>
    /// <returns>A VSSolution object containing the projects.</returns>
    public static VSSolution Parse(Stream stream)
    {
      if (stream.CanSeek && stream.Position != 0)
        stream.Position = 0;

      VSSolution solution = new VSSolution();
      using (StreamReader reader = new StreamReader(stream))
      {
        solution.ParseData(reader.ReadToEnd());
      }
      return solution;
    }
  }
}