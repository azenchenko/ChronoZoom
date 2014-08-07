/// <reference path="../scripts/authoring.ts" />
/// <reference path='../scripts/typings/jquery/jquery.d.ts'/>
/// <reference path='../ui/controls/listboxbase.ts'/>
var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var CZ;
(function (CZ) {
    (function (UI) {
        var TourListbox = (function (_super) {
            __extends(TourListbox, _super);
            function TourListbox(container, listItemContainer, contentItems, takeTour, editTour) {
                this.takeTour = takeTour;
                this.editTour = editTour;

                var listboxInfo = {
                    context: contentItems,
                    sortableSettings: null
                };

                var listItemsInfo = {
                    default: {
                        container: listItemContainer,
                        uiMap: {
                            closeButton: ".cz-listitem-close-btn",
                            iconImg: ".cz-contentitem-listitem-icon > img",
                            titleTextblock: ".cz-contentitem-listitem-title",
                            typeTextblock: ".cz-contentitem-listitem-highlighted"
                        }
                    }
                };

                listItemsInfo.default.ctor = TourListItem;
                _super.call(this, container, listboxInfo, listItemsInfo);
            }
            Object.defineProperty(TourListbox.prototype, "TakeTour", {
                get: function () {
                    return this.takeTour;
                },
                enumerable: true,
                configurable: true
            });

            Object.defineProperty(TourListbox.prototype, "EditTour", {
                get: function () {
                    return this.editTour;
                },
                enumerable: true,
                configurable: true
            });
            return TourListbox;
        })(UI.ListboxBase);
        UI.TourListbox = TourListbox;

        var TourListItem = (function (_super) {
            __extends(TourListItem, _super);
            function TourListItem(parent, container, uiMap, context) {
                if (!context)
                    throw "Tour list item's context is undefined";
                _super.call(this, parent, container, uiMap, context);

                this.iconImg = this.container.find(uiMap.iconImg);
                this.titleTextblock = this.container.find(uiMap.titleTextblock);
                this.descrTextblock = this.container.find(".cz-contentitem-listitem-descr");

                var self = this;
                var thumbUrl = this.data.thumbnailUrl;
                this.iconImg.attr("src", this.data.icon || "/images/Temp-Thumbnail2.png");
                var img = new Image();
                img.onload = function () {
                    self.iconImg.replaceWith(img);
                };
                img.onerror = function () {
                    if (console && console.warn)
                        console.warn("Could not load a thumbnail image " + thumbUrl);
                };
                img.src = thumbUrl; // fires off loading of image

                this.titleTextblock.text(this.data.title);
                if (this.data.description)
                    this.descrTextblock.text(this.data.description);
                else
                    this.descrTextblock.hide();

                this.container.find("#takeTour").click(function (e) {
                    parent.TakeTour(context);
                });

                container.find(".cz-tourslist-editing").css("display", parent.EditTour ? "inline" : "none");
                if (parent.EditTour) {
                    this.container.find("#editTour").click(function (e) {
                        parent.EditTour(context);
                    });
                }
            }
            return TourListItem;
        })(UI.ListItemBase);
        UI.TourListItem = TourListItem;
    })(CZ.UI || (CZ.UI = {}));
    var UI = CZ.UI;
})(CZ || (CZ = {}));
