// Copyright information can be found in the file named COPYING
// located in the root directory of this distribution.

#include "platform/platform.h"

#include "gui/core/guiControl.h"
#include "console/consoleTypes.h"
#include "T3D/gameBase/gameConnection.h"
#include "T3D/shapeBase.h"
#include "gfx/gfxDrawUtil.h"


//-----------------------------------------------------------------------------
/// A basic health bar control.
/// This gui displays the damage value of the current PlayerObjectType
/// control object.  The gui can be set to pulse if the health value
/// drops below a set value. This control only works if a server
/// connection exists and it's control object is a PlayerObjectType. If
/// either of these requirements is false, the control is not rendered.
class GuiHealthBarHud : public GuiControl
{
   typedef GuiControl Parent;

   bool     mShowFrame;
   bool     mShowFill;
   bool     mDisplayEnergy;

   ColorF   mFillColor;
   ColorF   mFrameColor;
   ColorF   mDamageFillColor;

   S32      mPulseRate;
   F32      mPulseThreshold;

   F32      mValue;

public:
   GuiHealthBarHud();

   void onRender( Point2I, const RectI &);
   static void initPersistFields();
   DECLARE_CONOBJECT( GuiHealthBarHud );
   DECLARE_CATEGORY( "Gui Game" );
   DECLARE_DESCRIPTION( "A basic health bar. Shows the damage value of the current\n"
      "PlayerObjectType control object." );
};


//-----------------------------------------------------------------------------

IMPLEMENT_CONOBJECT( GuiHealthBarHud );

ConsoleDocClass( GuiHealthBarHud,
   "@brief A basic health bar. Shows the damage value of the current PlayerObjectType control object.\n\n"
   "This gui displays the damage value of the current PlayerObjectType control object. "
   "The gui can be set to pulse if the health value drops below a set value. "
   "This control only works if a server connection exists and it's control object "
   "is a PlayerObjectType. If either of these requirements is false, the control is not rendered.\n\n"
   
   "@tsexample\n"
		"\n new GuiHealthBarHud()"
		"{\n"
		"	fillColor = \"0.0 1.0 0.0 1.0\"; // Fills with a solid green color\n"
		"	frameColor = \"1.0 1.0 1.0 1.0\"; // Solid white frame color\n"
		"	damageFillColor = \"1.0 0.0 0.0 1.0\"; // Fills with a solid red color\n"
		"	pulseRate = \"500\";\n"
		"	pulseThreshold = \"0.25\";\n"
		"	showFill = \"true\";\n"
		"	showFrame = \"true\";\n"
		"	displayEnergy = \"false\";\n"
		"};\n"
   "@endtsexample\n\n"
   
   "@ingroup GuiGame\n"
);


GuiHealthBarHud::GuiHealthBarHud()
{
   mShowFrame = mShowFill = true;
   mDisplayEnergy = false;
   mFillColor.set(0, 0, 0, 0.5);
   mFrameColor.set(0, 1, 0, 1);
   mDamageFillColor.set(0, 1, 0, 1);

   mPulseRate = 0;
   mPulseThreshold = 0.3f;
   mValue = 0.2f;
}

void GuiHealthBarHud::initPersistFields()
{
   addGroup("Colors");		
   addField( "fillColor", TypeColorF, Offset( mFillColor, GuiHealthBarHud ), "Standard color for the background of the control." );
   addField( "frameColor", TypeColorF, Offset( mFrameColor, GuiHealthBarHud ), "Color for the control's frame." );
   addField( "damageFillColor", TypeColorF, Offset( mDamageFillColor, GuiHealthBarHud ), "As the health bar depletes, this color will represent the health loss amount." );
   endGroup("Colors");		

   addGroup("Pulse");		
   addField( "pulseRate", TypeS32, Offset( mPulseRate, GuiHealthBarHud ), "Speed at which the control will pulse." );
   addField( "pulseThreshold", TypeF32, Offset( mPulseThreshold, GuiHealthBarHud ), "Health level the control must be under before the control will pulse." );
   endGroup("Pulse");		

   addGroup("Misc");		
   addField( "showFill", TypeBool, Offset( mShowFill, GuiHealthBarHud ), "If true, we draw the background color of the control." );
   addField( "showFrame", TypeBool, Offset( mShowFrame, GuiHealthBarHud ), "If true, we draw the frame of the control." );
   addField( "displayEnergy", TypeBool, Offset( mDisplayEnergy, GuiHealthBarHud ), "If true, display the energy value rather than the damage value." );
   endGroup("Misc");

   Parent::initPersistFields();
}


//-----------------------------------------------------------------------------
/**
   Gui onRender method.
   Renders a health bar with filled background and border.
*/
void GuiHealthBarHud::onRender(Point2I offset, const RectI &updateRect)
{
   // Must have a connection and player control object
   GameConnection* conn = GameConnection::getConnectionToServer();
   if (!conn)
      return;
   ShapeBase* control = dynamic_cast<ShapeBase*>(conn->getControlObject());
   if (!control || !(control->getTypeMask() & PlayerObjectType))
      return;

   if(mDisplayEnergy)
   {
      mValue = control->getEnergyValue();
   }
   else
   {
      // We'll just grab the damage right off the control object.
      // Damage value 0 = no damage.
      mValue = 1 - control->getDamageValue();
   }


   // Background first
   if (mShowFill)
      GFX->getDrawUtil()->drawRectFill(updateRect, mFillColor);

   // Pulse the damage fill if it's below the threshold
   if (mPulseRate != 0)
      if (mValue < mPulseThreshold) 
      {
         U32 time = Platform::getVirtualMilliseconds();
         F32 alpha = 2.0f * F32(time % mPulseRate) / F32(mPulseRate);
         mDamageFillColor.alpha = (alpha > 1.0f)? 2.0f - alpha: alpha;
      }
      else
         mDamageFillColor.alpha = 1;

   // Render damage fill %
   RectI rect(updateRect);
   if(getWidth() > getHeight())
      rect.extent.x = (S32)(rect.extent.x * mValue);
   else
   {
      S32 bottomY = rect.point.y + rect.extent.y;
      rect.extent.y = (S32)(rect.extent.y * mValue);
      rect.point.y = bottomY - rect.extent.y;
   }
   GFX->getDrawUtil()->drawRectFill(rect, mDamageFillColor);

   // Border last
   if (mShowFrame)
      GFX->getDrawUtil()->drawRect(updateRect, mFrameColor);
}
