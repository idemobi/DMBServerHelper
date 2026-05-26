# XML Documentation Rules (Game-Data-Forge)

## General Goals
- Generate complete and high-quality XML documentation for C# code
- Documentation must be understandable by junior developers
- Be explicit, educational, and clear

## Style
- Use full sentences
- Be verbose when needed (do not be overly short)
- Explain WHY the code exists, not only WHAT it does

## Required Tags
- Always use <summary>
- Use <param> for each parameter
- Use <returns> when applicable
- Use <remarks> for deeper explanations

## Content Rules
- Avoid generic phrases like "Gets or sets"
- Explain business purpose and context
- Clarify side effects (network, DB, cache, etc.)
- Mention async behavior explicitly
- Describe data flow when relevant

## Project Context
- The system uses "Agents" (AuthAgent, SyncAgent, etc.)
- Data types like PlayerData, Organization, etc. have business meaning
- Always explain the role of the method/class in the system

## Tone
- Pedagogical (explain like to a junior developer)
- Precise and structured
- Professional

## Special Requirement
- When referencing other classes, use <see cref="ClassName"/>