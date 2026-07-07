# Saving Games

In order to save game progress, it needs to be serialised and stored in the database.
For this, we've decided to use the [MemoryPack](https://github.com/Cysharp/MemoryPack)
library, which is a fast serialiser for C# with a binary format.

In this game, saving means saving the stats that persist across rounds. The actual
layout of the map is not saved since it changes every round. The game is only saved
in between rounds.

The actual data to be serialised is the `Stats` struct in [PlayerVariables.cs](../util/PlayerVariables.cs).

After serialisation, it is then dumped into the database into the `data` column of the
`saves` table, which has the type `BLOB` so we can store arbitrary binary data inside it.

When loading a game, the reverse is done by reading the blob from the DB and deserialising
it with MemoryPack.
