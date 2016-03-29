using System.Collections.Generic;
using System.Drawing;

namespace TesSaveLocationTracker.Tes.Renderer
{
    public struct CharacterSaves
    {
        public IEnumerable<TesSavegame> Saves { get; set; }

        public Brush Brush { get; set; }

        public string CharacterName { get; set; }
    }
}
