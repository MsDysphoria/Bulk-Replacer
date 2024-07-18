using System;
using System.Collections.Generic;
using System.IO;

namespace Bulk_Replacer;

public class Replacer
{
    public event Action<string> onError; 

    private ReplaceType type;
    private string toFind;
    private string replaceWith;
    private string path;
    private bool recursive;
    private bool caseSensitive;

    private Action<string> loggingAction;
    private int filesProcessed = 0;
    private int filesSkipped = 0;
    private List<string> processedFiles = new List<string>();

    public int FilesProcessedCount
    {
        get => filesProcessed;
    }
    
    public int FilesSkippedCount
    {
        get => filesSkipped;
    }
    
    public List<string> ProcessedFilesList
    {
        get => processedFiles;
    }
    
    public Replacer Type(ReplaceType type)
    {
        this.type = type;
        return this;
    }

    public Replacer Find(string text)
    {
        this.toFind = text;
        return this;
    }

    public Replacer Replace(string text)
    {
        this.replaceWith = text;
        return this;
    }

    public Replacer In(string path)
    {
        this.path = path;
        return this;
    }

    public Replacer Options(bool recursive, bool caseSensitive)
    {
        this.recursive = recursive;
        this.caseSensitive = caseSensitive;
        return this;
    }

    public Replacer OnLog(Action<string> loggingAction)
    {
        this.loggingAction = loggingAction;
        return this;
    }

    public void Execute()
    {
        if (string.IsNullOrEmpty(toFind) || "Pick a keyword to find".Equals(toFind))
        {
            onError?.Invoke("Please specify the keyword to find");
            return;
        }
        
        if ("Pick a keyword to replace with".Equals(replaceWith))
        {
            onError?.Invoke("Please specify the keyword to replace with.");
            return;
        }
        
        if (string.IsNullOrEmpty(path) || "Pick the target directory".Equals(path))
        {
            onError.Invoke("Please select a directory.");
            return;
        }
        
        IEnumerable<string> files = this.recursive
            ? Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)
            : Directory.GetFiles(path);

        loggingAction?.Invoke($"Processing files in {path} ({(recursive ? "including subfolders" : "without subfolders")})");
        loggingAction?.Invoke($"Find text: {toFind}");
        loggingAction?.Invoke($"Replace text: {replaceWith}");
        loggingAction?.Invoke("---");

        foreach (string file in files)
        {
            string fileName = System.IO.Path.GetFileName(file);
            string fileExtension = System.IO.Path.GetExtension(file);

            if (ReplaceType.Content == type)
            {
                ReplaceContent(file, fileName, fileExtension);
            }
            else if (ReplaceType.Extension == type)
            {
                ReplaceExtension(file, fileName, fileExtension);
            }
            else if (ReplaceType.Content == type)
            {
                ReplaceContent(file, fileName);
            }
        }
        
        loggingAction?.Invoke("---");
        loggingAction?.Invoke($"Total files processed: {filesProcessed}");
        loggingAction?.Invoke($"Total files skipped: {filesSkipped}");
    }

    private void ReplaceContent(string file, string fileName)
    {
        string fileContent = System.IO.File.ReadAllText(file);
        var casing = caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
        if (fileContent.Contains(toFind, casing))
        {
            string newContent = fileContent.Replace(toFind, replaceWith, casing);
            System.IO.File.WriteAllText(file, newContent);
            loggingAction?.Invoke($"File content updated: {fileName}");
            filesProcessed++;
        }
        else
        {
            filesSkipped++;
        }
    }

    private void ReplaceExtension(string file, string fileName, string fileExtension)
    {
        var casing = caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
        if (fileExtension.Contains(toFind, casing))
        {
            string newFileExtension = fileExtension.Replace(toFind, replaceWith, casing);

            if (string.IsNullOrWhiteSpace(newFileExtension))
            {
                loggingAction?.Invoke($"Warning: Skipping file {fileName} because the new extension is empty.");
                filesProcessed++;
                return;
            }

            string newFileName = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(file),
                System.IO.Path.GetFileNameWithoutExtension(file) + newFileExtension);
            System.IO.File.Move(file, newFileName);
            loggingAction?.Invoke($"File extension changed: {fileName} -> {System.IO.Path.GetFileName(newFileName)}");
            filesProcessed++;
            processedFiles.Add(newFileName);
        }
        else
        {
            filesProcessed++;
        }
    }

    private void ReplaceContent(string file, string fileName, string fileExtension)
    {
        var casing = caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
        if (fileName.Contains(toFind, casing))
        {
            string fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(fileName);
            string newFileName = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(file), fileNameWithoutExtension.Replace(toFind, replaceWith, casing) + fileExtension);
            
            System.IO.File.Move(file, newFileName);
            loggingAction?.Invoke($"File renamed: {fileName} -> {System.IO.Path.GetFileName(newFileName)}");
            filesProcessed++;
            processedFiles.Add(newFileName);
        }
        else
        {
            filesSkipped++;
        }
    }

    public enum ReplaceType
    {
        FileName = 0,
        Extension = 1,
        Content = 2
    }
}