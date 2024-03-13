Two collaborating systems: 
* Web-API with Subscribers
* Advertisment system that "uses" the Web-API

The Advertisement system handles Subsrcibers and Companies that want to create ads in the imaginary newspaper. The Subscriber Web-API has information about the Subscribers (people reading the newspaper).

The ER-Diagram below show the relationsship between Subscribers, Ads and Advertisers. An Ad is either connected (has a Foreign key) to a Subscriber or an Advertiser.

![image](https://github.com/mikael-anttila-eriksson/AdvertisementSystem/assets/105818456/3840de90-352b-4cb6-b915-3609ccca4d68)

The users (Subscriber or a Advertiser) path through the Advertisment system is represented in the flowchart below. On the first page (New Announcement) the user selects if it is a Subscriber or Advertiser. Then the user can fill in / controll its information and finally create the Ad.

![image](https://github.com/mikael-anttila-eriksson/AdvertisementSystem/assets/105818456/9cb8e789-b92f-4f6e-9f7d-db346755d973)
