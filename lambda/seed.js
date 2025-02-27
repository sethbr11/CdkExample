const AWS = require("aws-sdk");
const dynamo = new AWS.DynamoDB.DocumentClient();
const TABLE_NAME = process.env.TABLE_NAME;

exports.handler = async () => {
  const items = [
    {
      id: "1",
      name: "Original Glazed",
      description:
        "Our original glazed donut is a classic. It's a yeast-raised donut that's light and fluffy, and coated in a thin layer of our signature glaze.",
      image: "/images/og-glazed.png",
      alt: "The PDC Original Glazed Donut",
      loveCount: 8,
    },
    {
      id: "2",
      name: "Chocolate Glazed",
      description:
        "Our chocolate glazed donut is a classic. It's a yeast-raised donut that's light and fluffy, and coated in a thin layer of our signature chocolate glaze.",
      image: "/images/chocolate-glazed.png",
      alt: "The PDC Chocolate Glazed Donut",
      loveCount: 5,
    },
    {
      id: "3",
      name: "Cinnamon Roll",
      description:
        "Wait, that's not a donut! It's our signature cinnamon roll—one of our most popular items! A delectable treat to say the yeast.",
      image: "/images/cinnamon-roll.png",
      alt: "The PDC cinnamon roll",
      loveCount: 25,
    },
    {
      id: "4",
      name: "Strawberry with Sprinkles",
      description:
        "Can't go wrong with this donut. You looked at it and probably thought of the Simpsons, right? Tastes as good as it looks!",
      image: "/images/strawberry-sprinkles.png",
      alt: "The PDC Strawberry Donut with Sprinkles",
      loveCount: 5,
    },
    {
      id: "5",
      name: "Chocolate Custard Filled",
      description:
        "This donut is filled with our signature custard and coated in a thin layer of chocolate glaze. It's a fan favorite!",
      image: "/images/custard-filled-chocolate.png",
      alt: "The PDC Chocolate Custard Filled Donut",
      loveCount: 27,
    },
    {
      id: "6",
      name: "Chocolate Kreme Filled",
      description:
        "If you couldn't tell already, all of our donuts were stolen from Krispy Kreme. They'll never catch us. Try this one out!",
      image: "/images/kreme-filled-chocolate.png",
      alt: "The PDC Kreme-Filled Chocolate-Covered Donut",
      loveCount: 5,
    },
    {
      id: "7",
      name: "Lemon Filled",
      description:
        "Honestly, we don't know of anyone who likes this donut except our owner, and that's why we have it. It's just okay.",
      image: "/images/lemon-filled.png",
      alt: "The PDC Lemon Filled Donut",
      loveCount: -1,
    },
    {
      id: "8",
      name: "Cake Batter Filled",
      description:
        "This donut is filled with cake batter and coated in a thin layer of our signature glaze. Yep, you read that right! Sounds weird, but it's surprisingly good.",
      image: "/images/cake-batter-filled.png",
      alt: "The PDC Cake Batter Filled Donut",
      loveCount: 6,
    },
    {
      id: "9",
      name: "Raspberry Filled",
      description:
        "This donut is filled with raspberry jelly and coated in a thin layer of our signature glaze. The raspberries are even local! Local to someone we're sure...",
      image: "/images/raspberry-filled.png",
      alt: "The PDC Raspberry Filled Donut",
      loveCount: 5,
    },
    {
      id: "10",
      name: "Thai Young Coconut",
      description:
        "So, Krispy Kreme didn't have as many flavors as we thought, so we stole this from elsewhere. This donut is made with Thai young coconuts!",
      image: "/images/thai-young-coconut.jpg",
      alt: "The PDC Thai Young Coconut Donut",
      loveCount: 5,
    },
    {
      id: "11",
      name: "Blueberry Lavender",
      description:
        "One day, a free-spirited traveler traded this recipe for a dozen glazed. We tried it, laughed, and now it’s on our menu.",
      image: "/images/blueberry-lavender.jpg",
      alt: "The PDC Blueberry Lavender Donut",
      loveCount: 5,
    },
    {
      id: "12",
      name: "Ferrero Rocher",
      description:
        "So, Ferrero Rocher is one of those bougie brands and we thought it would be good on a donut. Surprisingly average, we won't even lie to you.",
      image: "/images/ferrero-rocher.jpg",
      alt: "The PDC Ferrero Rocher Donut",
      loveCount: 5,
    },
  ];

  for (const item of items) {
    const params = {
      TableName: TABLE_NAME,
      Item: item,
    };
    await dynamo.put(params).promise();
  }

  return {
    statusCode: 200,
    body: JSON.stringify({ message: "Table seeded successfully" }),
  };
};
