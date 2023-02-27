using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomNumerationPlugin
{
    [Transaction(TransactionMode.Manual)]
    public class RoomNumeration : IExternalCommand

    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            var rooms = new FilteredElementCollector(doc)
                     .OfCategory(BuiltInCategory.OST_Rooms)
                     .OfType<Room>()
                     .ToList();

            Transaction transaction = new Transaction(doc);
            transaction.Start("Room numeration");

            foreach (var room in rooms)
            {
                RoomTag newTag = CreateRoomTag(doc, room);
            }

            transaction.Commit();

            return Result.Succeeded;

        }

        private RoomTag CreateRoomTag(Document doc, Room room)
        {
            XYZ roomCenter = GetElementCenter(room);
            UV roomTagLocation = new UV(roomCenter.X, roomCenter.Y);

            LinkElementId roomId = new LinkElementId(room.Id);
            RoomTag roomTag = doc.Create.NewRoomTag(roomId, roomTagLocation, null);
            if (roomTag == null)
            {
                throw new Exception("Create a new room tag failed.");
            }

            return roomTag;
        }

        public XYZ GetElementCenter(Element element)
        {
            BoundingBoxXYZ bounding = element.get_BoundingBox(null);
            return (bounding.Max + bounding.Min) / 2;
        }


    }
}


