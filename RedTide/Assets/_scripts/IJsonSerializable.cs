using Lib.SimpleJSON;

public interface IJsonSerializable
{
    JSONNode SaveToJSON();

    void ReadFromJSON(JSONNode node);

}