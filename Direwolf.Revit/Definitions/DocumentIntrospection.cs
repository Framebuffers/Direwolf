using Autodesk.Revit.DB;

namespace Direwolf.Revit.Definitions;
/*
 * Author: Framebuffer
 * Date  : 2025-03-20
 * Direwolf Query Architecture
 *
 * I need to explain a couple of things to *understand* some of my decisions within Direwolf.
 *
 * To get Elements from Revit, there's an API feature called FilteredElementCollector (and others).
 * They make it super easy (and efficient) to get data back and forth Revit.
 * They use a small record attached to each Element to iterate quickly on each.
 * These are called FastFilters. Those who need to open the whole Element to check for some condition are called SlowFilters.
 *
 * They're great to run any kind of *specific* check.
 * However, for the main Reaper, there's a problem: there's several checks, sortings and conditionals to run *for each Element*.
 * Running a Filter for each condition would iterate through the entire record DB: even if it's fast, it's unnecessary.
 *
 * This is why the main Reaper has an ugly 400+ LOC function to sieve filters and sort them *manually*.
 * These extensions take some of those checks and make them actually usable.
 *
 * One more thing: but Frame, what does Direwolf do for me? This is where Introspection comes.
 * I know from personal experience what we needed and we didn't down in the modelling battleground.
 * It is *super* easy to reap data for data's sake. In fact, one approach is to serialise the *entire* document to Prey and send them to a PostgreSQL database. You can do that. There's a Howl for that.
 *
 * But I don't want to do that. This is what I'm going to do:
 * The thing I did the most was filling Parameters: how many were empty, how many were full, how much of something, how long until it's done...
 * And, apart from that, the secondary goal is making the *more annoying* sides of Revit less so.
 * Yes, things like View/Object Styles, checking if there were some hidden Element that was clogging the model, checking if the drawing was modelled or it was just made of lines and fills-- you know, because, again, some *common operations* for us modellers are just outright hostile.
 *
 * Introspection works by querying Elements in such a way where Revit gives a valid range of Elements, Direwolf sieves them by: CategoryType, Category, Family, Element, a Dictionary of Parameters. Direwolf *abstracts* these to records, just like Revit, to the important bits, whilst also leaving room to add more.
 *
 * Documents are extended with ready-to-go Filters for the most common operations, like the ones described above. Elements are extended with a method to get the Element's important info as a special kind of Prey, tailor-made to get just the important bits-- it's just like Revit, but applied to this specific use case.
 *
 * There are some legacy filters that may or may not work. These start with an underscore (_). The main Reaper will still work just like it was when I first started this project, but this should fix some bugs with invalid queries.
 *
 */

/// <summary>
///     Retrieves document metadata from a Revit document.
/// </summary>
/// <param name="Document">Revit document</param>
public readonly record struct DocumentIntrospection(Document Document)
{
    public string DocumentName => Document.Title;
    public string DocumentPath => Document.PathName;
    public string DocumentUniqueId => Document.CreationGUID.ToString();
    public string DocumentVersionId => Document.GetDocumentVersion(Document).VersionGUID.ToString();
    public int DocumentSaveCount => Document.GetDocumentVersion(Document).NumberOfSaves;
    public string[] Warnings => [.. Document.GetWarnings().Select(x => x.GetDescriptionText())];
    public double ActiveWorkset => Document.GetWorksetTable().GetActiveWorksetId().IntegerValue;
    public ProjectInformationIntrospection ProjectInformation => new(Document);
}