using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanGenetics {

    public EyeColour _eyeColour;
    public enum EyeColour { Blue, LightBlue, DarkBlue, Green, DarkGreen, Hazel, DarkHazel, Amber, Brown, DarkBrown }

    public HairColour _hairColour;
    public enum HairColour { LightAshBlonde, LightGoldenBlonde, MediumAshBlonde, MediumGoldenBlonde, DarkCoolBlonde, DarkGoldenBlonde, LightestGoldenBrown, LightAshBrown, MediumCaramelBrown, DarkGoldenBrown, SoftBlack, BlueBlack, StrawberryBlonde, LightAuburn, MediumMahoganyBrown, DarkSpiceBrown, Silver}

    public SkinTone _skinTone;
    public enum SkinTone { lightest0, lightest1, lightest2, lightest3, lightest4, lightest5, light0, light1, light2, light3, light4, light5, medium0, medium1, medium2, medium3, medium4, medium5, dark0, dark1, dark2, dark3, dark4, dark5 }

    public GameObject _go;
    public Human _human;
    public GameObject _hair;
    public Clothes _clothes;
    
    public struct Clothes
    {
        public GameObject top;
        public GameObject bottom;
        public GameObject shoes;
        public GameObject hat;

        public Clothes (GameObject t, GameObject b, GameObject s, GameObject h)
        {
            top = t;
            bottom = b;
            shoes = s;
            hat = h;
        }
    }

    public HumanGenetics ()
    {
        
    }

    void ChooseClothes ()
    {
        if (_human._gender == Human.Gender.female)
        {
            _hair = _go.GetComponent<HumanController>().femaleHairs[Random.Range(0, _go.GetComponent<HumanController>().femaleHairs.Length)];
        }
        else
        {
            _hair = _go.GetComponent<HumanController>().maleHairs[Random.Range(0, _go.GetComponent<HumanController>().maleHairs.Length)];
        }
        _clothes.top = _go.GetComponent<HumanController>().Shirts[Random.Range(0, _go.GetComponent<HumanController>().Shirts.Length)];
        _clothes.top.GetComponent<SkinnedMeshRenderer>().material.color = Random.ColorHSV();
        _clothes.top.SetActive(true);
        _clothes.shoes = _go.GetComponent<HumanController>().shoes[Random.Range(0, _go.GetComponent<HumanController>().shoes.Length)];
        _clothes.shoes.SetActive(true);
        _clothes.bottom = _go.GetComponent<HumanController>().shorts[Random.Range(0, _go.GetComponent<HumanController>().shorts.Length)];
        _clothes.bottom.SetActive(true);
        _hair.SetActive(true);

    }

    public HumanGenetics(GameObject go, Human human)
    {
        _go = go;
        _human = human;
        _eyeColour = (EyeColour)Random.Range(0, 10);
        _hairColour = (HairColour)Random.Range(0, 17);
        _skinTone = (SkinTone)Random.Range(0, 24);
        ChooseClothes();
    }

    public HumanGenetics(GameObject go, Human human, int template)
    {
        _go = go;
        _human = human;
        _eyeColour = (EyeColour)Random.Range(0, 10);
        _hairColour = (HairColour)Random.Range(0, 17);
        ChooseClothes();
        switch (template)
        {
            case 0:
                {
                    _skinTone = SkinTone.light4;
                    break;
                }
            case 1:
                {
                    _skinTone = SkinTone.light0;
                    break;
                }
            case 2:
                {
                    _skinTone = SkinTone.light2;
                    break;
                }
            case 3:
                {
                    _skinTone = SkinTone.dark0;
                    break;
                }
            case 4:
                {
                    _skinTone = SkinTone.medium0;
                    break;
                }
        }
    }

    public HumanGenetics(GameObject go, Human human, Human father, Human mother)
    {
        _go = go;
        _human = human;
        HumanGenetics temp = BreedHumanGenetics(father._humanGenetics, mother._humanGenetics);
        _eyeColour = temp._eyeColour;
        _hairColour = temp._hairColour;
        _skinTone = temp._skinTone;
        ChooseClothes();
    }

    HumanGenetics BreedHumanGenetics (HumanGenetics father, HumanGenetics mother) //recursive breeding function
    {
        if (father._human._father != null)
        {
            father = BreedHumanGenetics(father._human._father._humanGenetics, father._human._mother._humanGenetics);
        }
        if (mother._human._mother != null)
        {
            mother = BreedHumanGenetics(mother._human._father._humanGenetics, mother._human._mother._humanGenetics);
        }
        HumanGenetics f = father;
        HumanGenetics m = mother;
        HumanGenetics result = new HumanGenetics
        {
            _eyeColour = BreedEyeColour(f._eyeColour, m._eyeColour),
            _hairColour = BreedHairColour(f._hairColour, m._hairColour),
            _skinTone = BreedSkinTone(f._skinTone, m._skinTone)
        };
        return result;
    }

    public Material GetSkinColour()
    {
        return GameController.instance.skinColours[(int)_skinTone];
    }

    SkinTone BreedSkinTone (SkinTone father, SkinTone mother)
    {
        int temp = Random.Range(0, 100);
        if (SimplifyColour(father) == SimplifyColour(mother))
        {
            if (temp < 50)
            {
                MiddleSkinTone(father, mother);
            }
            else if (temp < 50 + 25)
            {
                return father;
            }
            else return mother;
        } else
        {
            if (temp < 80)
            {
                MiddleSkinTone(father, mother);
            } else if (temp < 80 + 10)
            {
                return father;
            } else
            {
                return mother;
            }
        }
        return MiddleSkinTone(father, mother);
    }

    SkinTone SimplifyColour (SkinTone s)
    {
        if ((int)s < 6)
        {
            return SkinTone.lightest0;
        } else if ((int)s < 12)
        {
            return SkinTone.light0;
        } else if ((int)s < 18)
        {
            return SkinTone.medium0;
        } else
        {
            return SkinTone.dark0;
        }
    }

    SkinTone MiddleSkinTone (SkinTone father, SkinTone mother)
    {
        int temp = Random.Range(0, 100);
        if (temp < 60)
        {
            return (SkinTone)Mathf.Round(((int)father + (int)mother) / 2);
        } else if (temp < 60 + 20)
        {
            return father;
        } else
        {
            return mother;
        }
    }

    public Material GetHairColour ()
    {
        return GameController.instance.hairColours[(int)_hairColour];
    }

    HairColour BreedHairColour(HairColour father, HairColour mother)
    {
        father = SimplifyColour(father);
        mother = SimplifyColour(mother);
        int temp = Random.Range(0, 100);
        if (father == mother)
        {
            if (father == HairColour.LightAshBlonde) //blonde-blonde
            {
                if (temp < 75) //75% chance for blonde
                {
                    return DesimplifyColour(father);
                } else if (temp < 75 + 10) //10% chance for brown
                {
                    return DesimplifyColour(HairColour.LightestGoldenBrown);
                } else if (temp < 75 + 10 + 10) //10% chance for ginger
                {
                    return DesimplifyColour(HairColour.StrawberryBlonde);
                } else //5% chance for silver
                {
                    return DesimplifyColour(HairColour.Silver);
                }
            } else if (father == HairColour.LightestGoldenBrown) //brown-brown
            {
                if (temp < 75) //75% chance for brown
                {
                    return DesimplifyColour(father);
                } else if (temp < 75 + 10) //10% chance for blonde
                {
                    return DesimplifyColour(HairColour.LightAshBlonde);
                } else if (temp < 75 + 10 + 10) //10% chance for ginger
                {
                    return DesimplifyColour(HairColour.StrawberryBlonde);
                } else //5% chance for silver
                {
                    return DesimplifyColour(HairColour.Silver);
                }
            } else if (father == HairColour.StrawberryBlonde) //ginger-ginger
            {
                if (temp < 75) //75% chance for ginger
                {
                    return DesimplifyColour(father);
                } else if (temp < 75 + 10) //10% chance for blonde
                {
                    return DesimplifyColour(HairColour.LightAshBlonde);
                } else if (temp < 75 + 10 + 10) //10% chance for brown
                {
                    return DesimplifyColour(HairColour.LightestGoldenBrown);
                } else //5% chance for silver
                {
                    return DesimplifyColour(HairColour.Silver);
                }
            } else if (father == HairColour.Silver)
            {
                return DesimplifyColour(father);
            }
        } else if (father == HairColour.LightAshBlonde || mother == HairColour.LightAshBlonde) //blonde-?
        {
            if (father == HairColour.LightestGoldenBrown || mother == HairColour.LightestGoldenBrown) //blonde-brown
            {
                if (temp < 50) //50% chance for brown
                {
                    return DesimplifyColour(HairColour.LightestGoldenBrown);
                } else if (temp < 50 + 30) //30% chance for blonde
                {
                    return DesimplifyColour(HairColour.LightAshBlonde);
                } else if (temp < 50 + 30 + 15) //15% chance for ginger
                {
                    return DesimplifyColour(HairColour.StrawberryBlonde);
                } else
                {
                    return DesimplifyColour(HairColour.Silver);
                }
            } else if (father == HairColour.StrawberryBlonde || mother == HairColour.StrawberryBlonde) //blonde-ginger
            {
                if (temp < 50) //50% chance for blonde 
                {
                    return DesimplifyColour(HairColour.LightAshBlonde);
                } else if (temp < 50 + 35) //35% chance for ginger
                {
                    return DesimplifyColour(HairColour.StrawberryBlonde);
                } else if (temp < 50 + 35 + 10) //10% chance for brown
                {
                    return DesimplifyColour(HairColour.LightestGoldenBrown);
                } else
                {
                    return DesimplifyColour(HairColour.Silver);
                }
            } else //blonde-silver
            {
                if (temp < 50) //50% chance for blonde
                {
                    return DesimplifyColour(HairColour.LightAshBlonde);
                } else if (temp < 50 + 40) //40% chance for silver
                {
                    return DesimplifyColour(HairColour.Silver);
                } else if (temp < 50 + 45) //5% chance for ginger
                {
                    return DesimplifyColour(HairColour.StrawberryBlonde);
                } else //5% chance for brown 
                {
                    return DesimplifyColour(HairColour.LightestGoldenBrown);
                }
            }
        } else if (father == HairColour.LightestGoldenBrown || mother == HairColour.LightestGoldenBrown) //brown-?
        {
            if (father == HairColour.StrawberryBlonde || mother == HairColour.StrawberryBlonde) //brown-ginger
            {
                if (temp < 50) //50% chance for ginger
                {
                    return DesimplifyColour(HairColour.StrawberryBlonde);
                } else if (temp < 50 + 35) //35% chance for brown
                {
                    return DesimplifyColour(HairColour.LightestGoldenBrown);
                } else if (temp < 50 + 35 + 10) //10% chance for blonde
                {
                    return DesimplifyColour(HairColour.LightAshBlonde);
                } else //5% chance for silver
                {
                    return DesimplifyColour(HairColour.Silver);
                }
            } else //brown-silver
            {
                if (temp < 50) //50% chance for silver
                {
                    return DesimplifyColour(HairColour.Silver);
                } else if (temp < 50 + 30)//30% chance for brown
                {
                    return DesimplifyColour(HairColour.LightestGoldenBrown);
                } else if (temp < 50 + 30 + 10) //10% chance for ginger
                {
                    return DesimplifyColour(HairColour.StrawberryBlonde);
                } else //10% chance for blonde
                {
                    return DesimplifyColour(HairColour.LightAshBlonde);
                }
            }
        } else if (father == HairColour.StrawberryBlonde || mother == HairColour.StrawberryBlonde) //ginger-silver
        {
            if (father == HairColour.Silver || mother == HairColour.Silver)
            {
                if (temp < 50) //50% chance for silver
                {
                    return DesimplifyColour(HairColour.Silver);
                } else if (temp < 50 + 35) //35% chance for ginger
                {
                    return DesimplifyColour(HairColour.StrawberryBlonde);
                } else if (temp < 50 + 35 + 10) //10% chance for brown
                {
                    return DesimplifyColour(HairColour.LightestGoldenBrown);
                } else //5% chance for blonde
                {
                    return DesimplifyColour(HairColour.LightAshBlonde);
                }
            }
        }
        return DesimplifyColour(HairColour.LightAshBlonde); //default
    }

    public Material GetEyeColour (EyeColour e)
    {
        return GameController.instance.eyeColours[(int)e];
    }

    public Material GetEyeColour ()
    {
        return GameController.instance.eyeColours[(int)_eyeColour];
    }

    EyeColour BreedEyeColour(EyeColour father, EyeColour mother)
    {
        father = SimplifyColour(father);
        mother = SimplifyColour(mother);
        int temp = Random.Range(0, 100); //percentage chance
        if (father == mother) //same eyes
        {
            if (father == EyeColour.Brown) //brown-brown
            {
                if (temp < 75) //75% chance of brown
                {
                    return DesimplifyColour(EyeColour.Brown);
                }
                else if (temp < 75 + 19) //19% chance of green
                {
                    return DesimplifyColour(EyeColour.Green);
                }
                else //6% chance of blue
                {
                    return DesimplifyColour(EyeColour.Blue);
                }
            } else if (father == EyeColour.Blue) //blue-blue
            {
                if (temp < 99) //99% chance of blue
                {
                    return DesimplifyColour(EyeColour.Blue);
                } else //1% chance of green
                {
                    return DesimplifyColour(EyeColour.Green);
                }
            } else // green-green
            {
                if (temp < 75) //75% chance of green
                {
                    return DesimplifyColour(EyeColour.Green);
                } else if (temp < 75 + 24) //24% chance of blue
                {
                    return DesimplifyColour(EyeColour.Blue);
                } else //1% chance of brown
                {
                    return DesimplifyColour(EyeColour.Brown);
                }
            }
        } else if (father == EyeColour.Green || mother == EyeColour.Green)
        {
            if (father == EyeColour.Brown || mother == EyeColour.Brown) //green-brown
            {
                if (temp < 50) //50% chance of brown
                {
                    return DesimplifyColour(EyeColour.Brown);
                } else if (temp < 50 + 38) //38% chance of green
                {
                    return DesimplifyColour(EyeColour.Green);
                } else //12% chance of blue
                {
                    return DesimplifyColour(EyeColour.Blue);
                }
            } else if (father == EyeColour.Blue || mother == EyeColour.Blue) //green-blue
            {
                if (temp < 50) //50% chance of green
                {
                    return DesimplifyColour(EyeColour.Green);
                } else // 50% chance of blue
                {
                    return DesimplifyColour(EyeColour.Blue);
                }
            }
        } else //blue-brown
        {
            if (temp < 50)
            {
                return DesimplifyColour(EyeColour.Brown);
            } else
            {
                return DesimplifyColour(EyeColour.Blue);
            }
        }
        return EyeColour.Green; //default
    }

    EyeColour SimplifyColour (EyeColour e)
    {
        if ((int)e < 3)
        {
            return EyeColour.Blue;
        } else if ((int)e < 5)
        {
            return EyeColour.Green;
        } else
        {
            return EyeColour.Brown;
        }
    }

    HairColour SimplifyColour(HairColour h)
    {
        if ((int)h < 6)
        {
            return HairColour.LightAshBlonde;
        }
        else if ((int)h < 12)
        {
            return HairColour.LightestGoldenBrown;
        }
        else if ((int)h < 16)
        {
            return HairColour.StrawberryBlonde;
        }
        else
        {
            return HairColour.Silver;
        }
    }

    HairColour DesimplifyColour(HairColour h)
    {
        if (h == HairColour.LightAshBlonde)
        {
            int temp = Random.Range(0, 6);
            return (HairColour)temp;
        } else if (h == HairColour.LightestGoldenBrown)
        {
            int temp = Random.Range(0, 6) + 6;
            return (HairColour)temp;
        } else if (h == HairColour.StrawberryBlonde)
        {
            int temp = Random.Range(0, 4) + 12;
            return (HairColour)temp;
        } else
        {
            return h;
        }
    }

    EyeColour DesimplifyColour (EyeColour e)
    {
        if (e == EyeColour.Brown)
        {
            int temp = Random.Range(0, 5) + 5;
            return (EyeColour)temp;
        } else if (e == EyeColour.Blue)
        {
            int temp = Random.Range(0, 3);
            return (EyeColour)temp;
        } else
        {
            int temp = Random.Range(0, 2) + 3;
            return (EyeColour)temp;
        }
    }
}
