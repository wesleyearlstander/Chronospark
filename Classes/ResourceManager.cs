using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager
{
    //helper struct
    [System.Serializable]
    public struct Resources
    {
        public float amount;
        public ResourceType resourceType;

        public Resources (float a, ResourceType t)
        {
            amount = a;
            resourceType = t;
        }

        public override string ToString()
        {
            return resourceType.ToString() + ": " + amount.ToString();
        }
    }
    //helper enum for resource type
    public enum ResourceType
    {
        Food = 0, Wood = 1, Stone = 2
    }
    public Resources[] resource; //all the resources that are stored
    public Resources[] resourceMax;
    public List<Resource> resources; //all the resource deposits
    public int crops = 0;

    public ResourceManager()
    {
        resources = new List<Resource>();
        resource = new Resources[2];
        resourceMax = new Resources[2];
        Resources food, wood, foodMax, woodMax;
        foodMax = new Resources(2500, ResourceType.Food);
        woodMax = new Resources(500, ResourceType.Wood);
        food.resourceType = ResourceType.Food;
        food.amount = 2000;
        wood.resourceType = ResourceType.Wood;
        wood.amount = 0;
        resource[0] = food;
        resource[1] = wood;
        resourceMax[0] = foodMax;
        resourceMax[1] = woodMax;
    }

    public void AddResource (Resource resource)
    {
        resources.Add(resource);
        if (resource.resourceT == ResourceType.Food)
        {
            crops++;
        }
    }

    public int peopleOnCrops = 0;

    public void SetMaxFood (int humans)
    {
        resourceMax[0] = new Resources(humans * 500, ResourceType.Food);
    }

    public void RemoveResource (Resource resource)
    {
        resources.Remove(resource);
        bool temp = false;
        foreach (Resource r in resources)
        {
            if (r.resourceT == ResourceType.Wood) temp = true;
        }
        if (resource.resourceT == ResourceType.Food) crops--;
        if (!temp) EventHandler.instance.AddAchievement(Achievement.sponsored);
    }

    public float GetResources (ResourceType t) 
    {
        foreach (Resources r in resource)
        {
            if (r.resourceType == t)
            {
                return r.amount;
            }
        }
        return 0;
    }

    public void AddTime (float time)
    {
        Resource temp = null;
        foreach (Resource r in resources)
        {
            if (!r.dead)
            {
                if (r.AddTime(time))
                {
                    if (AddResources(r.GetResources()))
                    {

                    }
                    else
                    {
                        //resource doesn't exist
                    }
                }
            } else
            {
                temp = r;
            }
        }
        if (temp != null)
            RemoveResource(temp);
    }


    public bool AddResources(Resources resources) //returns false if new resource or resource doesnt exist yet
    {
        int count = 0;
        int length = resource.Length;
        while (count < length)
        {
            if (resource[count].resourceType == resources.resourceType)
            {
                resource[count].amount = Mathf.Clamp(resource[count].amount + resources.amount, 0, resourceMax[count].amount);
                if (resources.resourceType == ResourceType.Wood && resource[count].amount >= 100)
                {
                    EventHandler.instance.AddHiddenEvent(EventHandler.EventType.GatheredWoodForHouse);
                }
                return true;
            }
            count++;
        }
        return false;
    }

    public bool AddNewResources(Resources newResources) //returns false if resource already exists
    {
        Resources[] temp = new Resources[resource.Length + 1];
        Resources[] tempMax = new Resources[resourceMax.Length + 1];
        int count = 0;
        foreach (Resources r in resource)
        {
            if (r.resourceType == newResources.resourceType)
                return false;
            temp[count] = r;
            count++;
        }
        temp[count] = newResources;
        count = 0;
        foreach (Resources r in resource)
        {
            tempMax[count] = r;
            count++;
        }
        tempMax[count] = new Resources(2500, newResources.resourceType);
        resource = temp;
        return true;
    }

    public bool EnoughResources (Resources resources) //single resource check
    {
        foreach (Resources r in this.resource)
        {
            if (r.resourceType == resources.resourceType)
            {
                if (r.amount >= resources.amount)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool EnoughResources (Resources [] resources) //overloaded method for multiple resources
    {
        int count = 0;
        foreach (Resources e in resources)
        {
            if (EnoughResources(e))
            {
                count++;
            }
        }
        if (count == resources.Length)
        {
            return true;
        }
        return false;
    }

    public bool DeductResources (Resources resources)
    {
        int count = 0;
        int length = resource.Length;
        if (EnoughResources(resources))
        {
            while (count < length)
            {
                if (resource[count].resourceType == resources.resourceType)
                {
                    if (resources.resourceType == ResourceType.Food && GameController.instance.GetComponent<MovieController>().isActiveAndEnabled && resource[count].amount < 600)
                        AddResources(new Resources(100, ResourceType.Food));
                    resource[count].amount = Mathf.Clamp(resource[count].amount - resources.amount, 0, resourceMax[count].amount);
                    return true;
                }
                count++;
            }
        }
        return false;
    }

    public bool DeductResources (Resources[] resources)
    {
        int count = 0;
        int length = resources.Length;
        if (EnoughResources(resources))
        {
            while (count < length)
            {
                if (DeductResources(resources[count]))
                {
                    count++;
                }
            }
            if (count == length)
            {
                return true;
            }
        }
        return false;
    }


}
