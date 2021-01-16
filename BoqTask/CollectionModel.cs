using System.Collections.Generic;

public class Data
{
    public string name { get; set; }
    public string value { get; set; }
}

public class Link
{
    public string rel { get; set; }
    public string href { get; set; }
}

public class Item
{
    public string href { get; set; }
    public List<Data> data { get; set; }
    public List<Link> links { get; set; }

    // In this scenario this will be for the topics associated with a session but can be used for nested objects in general
    public List<Item> items { get; set; } 

}

public class Collection
{
    public string version { get; set; }
    public List<object> links { get; set; }
    public List<Item> items { get; set; }
}

public class CollectionModel
{
    public Collection collection { get; set; }
}

