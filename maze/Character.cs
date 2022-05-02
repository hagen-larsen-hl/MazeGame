using System;
using Microsoft.Xna.Framework.Graphics;

namespace CS5410
{
    public class Character
    {
        public Cell cell;
        public Texture2D m_character;

        public Character(Cell cell, Texture2D m_character)
        {
            this.cell = cell;
            this.m_character = m_character;
        }
    }
}