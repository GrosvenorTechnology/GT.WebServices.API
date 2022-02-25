using GT.WebServices.API.Domain;
using System;
using System.Collections.Generic;

namespace GT.WebServices.API.Services
{
    public class DataCollectionService : IDataCollectionService
    {
        private DateTime _revision = DateTime.UtcNow;
        private DataCollection _dataCollection = LoadData();

        //We reset this on startup so the clock will only download the file
        // once per run as a demo
        public DateTime GetRevision()
        {
            return _revision;
        }

        public DataCollection GetData()
        {
            return _dataCollection;
        }

        private static DataCollection LoadData()
        {
            var data = new DataCollection
            {
                Flows = new List<DataCollectionFlow>
                {
                    new DataCollectionFlow {
                        Id = "absences",
                        Config = new Config {
                            Button = new Button {
                                Label = "Absences"
                            }
                        },
                        Levels = new List<Level> {
                            new Level {
                                Title = "Select Absence",
                                Items = new List<LevelItem> {
                                    new () {
                                        Id = "ABS001",
                                        Label = "Vacation"
                                    },
                                    new () {
                                        Id = "ABS002",
                                        Label = "Sick"
                                    },
                                    new () {
                                        Id = "ABS003",
                                        Label = "Doctor"
                                    },
                                    new () {
                                        Id = "ABS004",
                                        Label = "Dentist"
                                    },
                                    new () {
                                        Id = "ABS005",
                                        Label = "On business"
                                    }
                                }
                            }
                        }
                    },
                    new DataCollectionFlow { 
                        Id = "covid",
                        Config = new Config {
                            Button = new Button {
                                Label = "Covid (multi-level)"
                            }
                        },
                        Levels = new List<Level> {
                            new Level {
                                Title = "Do you have COVID?",
                                Items = new List<LevelItem> {
                                    new () {
                                        Id = "Yes",
                                        Label = "Yes",
                                        Levels = WrapSubLevels(new ()
                                        {
                                            new() {
                                                Id = "Temperature-Yes",
                                                Label = "Temperature - Yes",
                                                Levels = WrapSubLevels(new ()
                                                {
                                                    new() {
                                                        Id = "Cough-Yes",
                                                        Label = "Cough - Yes"
                                                    },
                                                    new() {
                                                        Id = "Cough-No",
                                                        Label = "Cough - No"
                                                    }
                                                })
                                            },
                                            new() {
                                                Id = "Temperature-No",
                                                Label = "Temperature - No"
                                            }
                                        })
                                    },
                                    new () {
                                        Id = "No",
                                        Label = "No"
                                    }
                                }
                            }
                        }
                    }
                }
            };

            return data;
        }

        private static SubLevels WrapSubLevels(List<LevelItem> items)
        {
            return new SubLevels
            {
                Level = new SubLevel
                {
                    Items = items
                }
            };
        }
    }
}

//**Simple example datacollection.xml**::

//    <?xml version = "1.0" encoding= "utf-8" ?>
//    < dataCollection >
//        < dataCollectionFlow id = "TF1" >
//          < config >
//            < button >
//              < label > Absence </ label >
//            </ button >
//          </ config >
//          < levels >
//            < level >
//              < title > Select Absence</title>
//              <items>
//                <item id = "ABS001" >
//                  < label > Vacation </ label >
//                </ item >
//                < item id= "ABS002" >
//                  < label > Sick </ label >
//                </ item >
//                < item id= "ABS003" >
//                  < label > Doctor </ label >
//                </ item >
//                < item id= "ABS004" >
//                  < label > Dentist </ label >
//                </ item >
//                < item id= "ABS005" >
//                  < label > On business</label>
//                </item>
//              </items>
//              </level>
//            </levels>
//        </dataCollectionFlow>
//    </dataCollection>

//.. important::
//    All* dataCollectionFlow*, * item* and* level* (except the first) elements must have 
//    an *id* attribute. 

//Text (button label, title, item label, etc) can be specified per language by using 
//*text * tags.The text tags can be omitted if only one language is required.

//Example::

//    <item id="ABS005">
//      <label>
//        <text language="en">On business</text>
//        <text language="de">Arbeitsreise</text>
//      </label>
//    </item>
    
//Configuration section
//~~~~~~~~~~~~~~~~~~~~~
    
//The configuration section can contain information used by the :ref:`action_ce.datacollection.menu`
//action for displaying a button for each flow. A label, role and group can be defined.The button
//will not be placed, if the employee does not have the role specified. The button will also not be 
//placed if a different group is defined for the action (see :ref:`action_ce.datacollection.menu`).

//Example button configuration::

//    <dataCollection>
//      <dataCollectionFlow id="TF1">
//        <config>
//          <button>
//            <label>Absence</label>
//            <group>absence</group>
//            <reqRole>fulltime</reqRole>
//          </button>
//        </config>
//        [...]
//      </ dataCollectionFlow >
//    </ dataCollection >

//The configuration section can also contain a response message, which is displayed after the transaction
//was successfully sent.

//Example response message configuration::

//    <dataCollection>
//      <dataCollectionFlow id="TF1">
//        <config>
//          <response>
//            <message>Accepted!</message>
//          </response>
//        </config>
//        [...]
//      </ dataCollectionFlow >
//    </ dataCollection >

//In addition to the response message the configuration section can also define whether the transaction is saved
//in the last clockings table allowing it to be reviewed at the terminal.

//Example of last clockings review configuration::

//    <dataCollection>
//      <dataCollectionFlow id="TF1">
//        <config>
//          <response>
//            <message>Accepted!</message>
//          </response>
//          <review>
//            <type>pabs</type>
//            <label>
//              <text language="en">P.ABS</text>
//            </label>
//          </review>          
//        </config>
//        [...]
//      </ dataCollectionFlow >
//    </ dataCollection >

//Nested levels
//~~~~~~~~~~~~~

//The simple example from above defines only one level with 5 items but it is
//also possible to configure nested and linked levels.

//With nested levels, each item can have sub-levels, so that a large selection can 
//be broken down into higher level groups that can be drilled down to refine the
//selection.

//Example::

//    <levels>
//      <level>
//        <items>
//          <item id="item1">
//            <label>Item 1</label>
//            <levels>
//              <level>
//                <items>
//                  <item id="item1-1">
//                      <label>Item 1-1</label>
//                  </item>
//                  <item id="item1-2">
//                      <label>Item 1-2</label>
//                  </item>
//                </items>
//              </level>
//            </levels>
//          </item>
//          <!-- more items -->
//        </items>
//      </level>
//    </levels>

//.. graphviz::
//    :caption: Nested items with 3 levels.

//    digraph tasks_nested
//{

//    items1 [shape="record", label="<i1> Item 1|<i2> Item 2"];
//    items2 [shape="record", label="<i11> Item 1-1|<i12> Item1-2"];
//    items3 [shape="record", label="<i21> Item 2-1|<i22> Item2-2"];

//    items1:i1:s -> items2:i11:n;
//    items1:i1:s -> items2:i12:n;
//    items1:i2:s -> items3:i21:n;
//    items1:i2:s -> items3:i22:n;

//    items4 [shape="record", label="<i111> Item 1-1-1|<i112> Item 1-1-2"];
//    items5 [shape="record", label="<i121> Item 1-2-1|<i122> Item 1-1-2"];
//    items6 [shape="record", label="<i211> Item 2-2-1|<i212> Item 2-1-2"];
//    items7 [shape="record", label="<i221> Item 2-2-1|<i222> Item 2-2-2"];

//    items2:i11:s -> items4:i111:n;
//    items2:i11:s -> items4:i112:n;
//    items2:i12:s -> items5:i121:n;
//    items2:i12:s -> items5:i122:n;
//    items3:i21:s -> items6:i211:n;
//    items3:i21:s -> items6:i212:n;
//    items3:i22:s -> items7:i221:n;
//    items3:i22:s -> items7:i222:n;

//}

//Multi - levels or linked levels
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

//With nested levels, each item defines its own sub-levels. Sometimes however,
//  the next level may not depend on the previous selection. Linked levels
//allow to reference a level, making it possible for many items to link to
//the same sub-level.

//Example::

//    <levels>
//      <level>
//        <items>
//          <item id="item1">
//            <label>Item 1</label>
//            <levelRef ref="level2" />
//          </item>
//          <item id="item2">
//            <label>Item 2</label>
//            <levelRef ref="level2" />
//          </item>
//        </items>
//      </level>
//      <level id="level2">
//        <items>
//          <item id="itemA">
//            <label>Item A</label>
//            <levelRef ref="level3" />
//          </item>
//          <item id="itemB">
//            <label>Item B</label>
//            <levelRef ref="level3" />
//          </item>
//        </items>
//      </level>
//      <!-- more levels -->
//    </levels>

//..  graphviz::
//    :caption: Multi - level items with 3 levels.

//    digraph tasks_linked {

//        items1 [shape="record", label="<i1> Item 1|<i2> Item 2"] ;
//items2[shape = "record", label = "<iA> Item A|<iB> Item B"];
//items3[shape = "record", label = "<iC> Item α|<iD> Item β"];

//items1: i1->items2:iA;
//items1: i1->items2:iB;
//items1: i2->items2:iA;
//items1: i2->items2:iB;
//items2: iA->items3:iC;
//items2: iA->items3:iD;
//items2: iB->items3:iC;
//items2: iB->items3:iD;

//    }

//Data entry
//~~~~~~~~~~

//An item can also define a data entry. Data entries can be used to 
//ask the user for entering additional data (e.g. amount,
//cost center ID, price, etc.).

//A data entry defines a sequence of entry steps. The following types
//of data entries are available:

// -numeric input
//- text input
//- masked text input

//Example for two numeric data entry steps::

//  <item id="apples">
//    <label>Apples</label>
//    <dataEntry>
//      <numericEntryStep id="apples.typeid">
//        <title>Enter type ID of apple</title>
//      </numericEntryStep>
//      <numericEntryStep id="apples.amount">
//        <title>How many apples</title>
//      </numericEntryStep>
//    </dataEntry>
//  </item>

//An item with a data entry can also have further sub-levels.

//Numeric data entry steps
//^^^^^^^^^^^^^^^^^^^^^^^^

//Example::

//  <numericEntryStep id="project.id">
//    <title>Project ID</title>
//    <default>1000</default>
//    <min>1000</min>
//    <max>9999</max>
//    <allowEmpty>true</allowEmpty>
//  </numericEntryStep>
  
//The example above defines a numeric entry step with a default,
//minimum and maximum value and specifies that an empty input
//is allowed.

//Text data entry steps
//^^^^^^^^^^^^^^^^^^^^^

//Example::

//  <item id="not-listed-project">
//    <label>Project not listed</label>
//    <dataEntry>
//      <textEntryStep id="not-listed-project.name">
//        <title>Project name</title>
//        <default>Prj-</default>
//        <min>3</min>
//        <max>10</max>
//      </textEntryStep>
//    </dataEntry>
//  </item>

//Specifying a default or a minimum and maximum length is optional.

//Masked data entry steps
//^^^^^^^^^^^^^^^^^^^^^^^

//Example::

//  <item id="not-listed-project">
//    <label>Project not listed</label>
//    <dataEntry>
//      <maskedEntryStep id="not-listed-project.prj-id">
//        <title>Project ID</title>
//        <mask>___-####</mask>
//        <default>PRJ-0000</default>
//      </maskedEntryStep>
//    </dataEntry>
//  </item>

//A masked data entry step allows for a fixed length entry in which
//allowed characters can be defined for each position.

//The meaning of the mask characters is as follows:

//  =============== =====================================
//  Mask Character Allowed Characters
//  =============== =====================================    
//  **#**           digits (0-9)
//  **A**           alphabetic and upper case (A-Z)
//  **a * *alphabetic and lower case (a-z)
//  **? **alphanumeric(a - z, A - Z, 0 - 9)
//  * *\***any
//   * *_ * *none(read only)
//   =============== =====================================

// Referencing data entries
//^^^^^^^^^^^^^^^^^^^^^^^^

//Referencing already defined data entries can be useful if many items
//within a level need to use the same data entry steps. 
//In these cases, the data entry within the item only references a data entry 
//by its ID. The actual data entries are defined with the level.

//Example::

//  <level>
//    <title>Select fruit</title>
//    <dataEntries>
//      <dataEntry id="fruit.amount">
//        <numericEntryStep id="amount">
//          <title>Enter amount</title>
//        </numericEntryStep>
//      </dataEntry>
//    </dataEntries>
    
//    <items>
//      <item id="fruit.apples">
//        <label>Apples</label>
//        <dataEntry ref="fruit.amount" />
//      </item>

//      <item id="fruit.bananas">
//        <label>Bananas</label>
//        <dataEntry ref="fruit.amount" />
//      </item>

//      <item id="fruit.oranges">
//        <label>Oranges</label>
//        <dataEntry ref="fruit.amount" />
//      </item>
//    </items>
//  </level>
