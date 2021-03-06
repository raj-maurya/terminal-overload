// Copyright information can be found in the file named COPYING
// located in the root directory of this distribution.

#include "platform/platform.h"
#include "platform/menus/menuBar.h"
#include "platform/menus/popupMenu.h"
#include "gui/core/guiCanvas.h"
#include "console/engineAPI.h"

//-----------------------------------------------------------------------------
// Constructor/Destructor
//-----------------------------------------------------------------------------

MenuBar::MenuBar()
{
   createPlatformPopupMenuData();

   mCanvas = NULL;
}

MenuBar::~MenuBar()
{
   removeFromCanvas();

   deletePlatformPopupMenuData();
}

IMPLEMENT_CONOBJECT(MenuBar);

ConsoleDocClass( MenuBar,
   "@brief Used for rendering platform menu bars\n\n"
   "Internal use only\n\n"
   "@internal"
);

//-----------------------------------------------------------------------------
// Public Methods
//-----------------------------------------------------------------------------

void MenuBar::addObject(SimObject *obj)
{
   Parent::addObject(obj);
   updateMenuBar(dynamic_cast<PopupMenu *>(obj));
}

void MenuBar::removeObject(SimObject *obj)
{
   Parent::removeObject(obj);
   updateMenuBar(dynamic_cast<PopupMenu *>(obj));
}

void MenuBar::insertObject(SimObject *obj, S32 pos)
{
   Parent::addObject(obj);
   
   if(pos >= size())
      pos = size() - 1;

   if(pos < size())
   {
      if(pos < 0) pos = 0;
      Parent::reOrder(obj, at(pos));
   }
   updateMenuBar(dynamic_cast<PopupMenu *>(obj));
}

void MenuBar::pushObject(SimObject *obj)
{
   Parent::pushObject(obj);
   updateMenuBar(dynamic_cast<PopupMenu *>(obj));
}

void MenuBar::popObject()
{
   Parent::popObject();
   updateMenuBar();
}

bool MenuBar::reOrder(SimObject *obj, SimObject *target /*= 0*/)
{
   bool ret = Parent::reOrder(obj, target);
   if(ret)
      updateMenuBar(dynamic_cast<PopupMenu *>(obj));
   return ret;
}

//-----------------------------------------------------------------------------
// Console Methods
//-----------------------------------------------------------------------------

DefineConsoleMethod(MenuBar, attachToCanvas, void, (const char *canvas, S32 pos), , "(GuiCanvas, pos)")
{
   object->attachToCanvas(dynamic_cast<GuiCanvas*>(Sim::findObject(canvas)), pos);
}

DefineConsoleMethod(MenuBar, removeFromCanvas, void, (), , "()")
{
   object->removeFromCanvas();
}

//-----------------------------------------------------------------------------

DefineConsoleMethod(MenuBar, insert, void, (SimObject* pObject, S32 pos), ,"(object, pos) insert object at position")
{

   if(pObject)
      object->insertObject(pObject, pos);
}
