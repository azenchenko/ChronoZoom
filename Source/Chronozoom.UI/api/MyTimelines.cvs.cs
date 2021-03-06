﻿using Chronozoom.Entities;
using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;


namespace Chronozoom.UI
{
    public partial class ChronozoomSVC: IChronozoomSVC
    {
        public Collection<TimelineShortcut> GetUserTimelines(string superCollection, string Collection)
        {
            return ApiOperation<Collection<TimelineShortcut>>(delegate(User user, Storage storage)
            {
                if (user == null) return null;

//#if RELEASE
//              if (user == null)
//              {
//                  return null;
//              }
//#endif

                Timeline roottimeline = GetTimelines(superCollection, Collection, null, null, null, null, null, null);

                var elements = new Collection<TimelineShortcut>();

                var timeline = storage.Timelines.Where(x => x.Id == roottimeline.Id)
                            .Include("Collection")
                            .Include("Collection.SuperCollection")
                            .Include("Collection.User")
                            .Include("Exhibits")
                            .Include("Exhibits.ContentItems")
                            .FirstOrDefault();

                if (timeline != null) elements.Add(storage.GetTimelineShortcut(timeline));

                if (roottimeline.ChildTimelines != null)
                {
                    foreach (var t in roottimeline.ChildTimelines)
                    {
                        timeline = storage.Timelines.Where(x => x.Id == t.Id)
                            .Include("Collection")
                            .Include("Collection.SuperCollection")
                            .Include("Collection.User")
                            .Include("Exhibits")
                            .Include("Exhibits.ContentItems")
                            .FirstOrDefault();

                        if (timeline != null)
                            elements.Add(storage.GetTimelineShortcut(timeline));
                    }
                }

                return elements;
            });
        }

        /// <summary>
        /// Documented under IChronozoomSVC
        /// </summary>
        /// <param name="includeMine"></param>
        /// <returns></returns>
        public Collection<TimelineShortcut> GetEditableTimelines(bool includeMine = false)
        {
            Trace.TraceInformation("GetEditableTimelines");
            return ApiOperation<Collection<TimelineShortcut>>(delegate(User user, Storage storage)
            {
                Collection<TimelineShortcut> rv = new Collection<TimelineShortcut>();

                List<Collection> collections;
                if (includeMine)
                {
                    // Collections where others can edit and user is in edit list, or user is collection owner.
                    collections = storage.Collections
                                    .Where(c => c.User.Id == user.Id || (c.Members.Any(m => m.User.Id == user.Id) && c.MembersAllowed))
                                    .Include("SuperCollection")
                                    .OrderByDescending(c => c.User.Id == user.Id)
                                    .ThenBy(c => c.User.DisplayName).ThenBy(c => c.Title)
                                    .ToList();
                }
                else
                {
                    // Collections where others can edit and user is in edit list, but user is not collection owner.
                    collections = storage.Collections
                                    .Where(c => c.Members.Any(m => m.User.Id == user.Id) && c.MembersAllowed && c.User.Id != user.Id)
                                    .Include("SuperCollection")
                                    .OrderBy(c => c.User.DisplayName).ThenBy(c => c.Title)
                                    .ToList();
                }

                foreach (Collection collection in collections)
                {
                    Collection<TimelineShortcut> collectionTimelines = GetUserTimelines(collection.SuperCollection.Title, collection.Title);
                    foreach (TimelineShortcut collectionTimeline in collectionTimelines)
                    {
                        rv.Add(collectionTimeline);
                    }
                }

                return rv;
            });
        }
    }
}