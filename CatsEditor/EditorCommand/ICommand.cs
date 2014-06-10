using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Editor;

namespace Catsland.CatsEditor.EditorCommand {
    public interface ICommand {
        bool Execute(MapEditor _mapEditor);
    }
}
