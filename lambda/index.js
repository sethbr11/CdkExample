const AWS = require("aws-sdk");
const dynamo = new AWS.DynamoDB.DocumentClient();
const TABLE_NAME = process.env.TABLE_NAME;

exports.handler = async (event) => {
  let response;
  try {
    switch (event.httpMethod) {
      case "GET":
        response = await getItems();
        break;
      case "POST":
        response = await createItem(JSON.parse(event.body));
        break;
      case "PUT":
        response = await updateLoveCount(JSON.parse(event.body));
        break;
      default:
        response = {
          statusCode: 400,
          body: JSON.stringify({ message: "Unsupported method" }),
        };
    }
  } catch (error) {
    response = {
      statusCode: 500,
      body: JSON.stringify({
        message: "Internal Server Error",
        error: error.message,
      }),
    };
  }

  // Add CORS headers to the response
  response.headers = {
    "Content-Type": "application/json",
    "Access-Control-Allow-Origin": "*",
    "Access-Control-Allow-Headers":
      "Content-Type,X-Amz-Date,Authorization,X-Api-Key",
    "Access-Control-Allow-Methods": "GET,POST,PUT",
  };

  return response;
};

const getItems = async () => {
  const params = {
    TableName: TABLE_NAME,
  };
  const data = await dynamo.scan(params).promise();
  return {
    statusCode: 200,
    body: JSON.stringify(data.Items),
  };
};

const createItem = async (item) => {
  const params = {
    TableName: TABLE_NAME,
    Item: item,
  };
  await dynamo.put(params).promise();
  return {
    statusCode: 201,
    body: JSON.stringify(item),
  };
};

const updateLoveCount = async (item) => {
  const params = {
    TableName: TABLE_NAME,
    Key: { id: item.id },
    UpdateExpression: "set loveCount = loveCount + :val",
    ExpressionAttributeValues: {
      ":val": 1,
    },
    ReturnValues: "UPDATED_NEW",
  };
  const data = await dynamo.update(params).promise();
  return {
    statusCode: 200,
    body: JSON.stringify(data.Attributes),
  };
};
