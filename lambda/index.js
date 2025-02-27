const AWS = require("aws-sdk");
const dynamo = new AWS.DynamoDB.DocumentClient();
const TABLE_NAME = process.env.TABLE_NAME;

exports.handler = async (event) => {
  switch (event.httpMethod) {
    case "GET":
      return await getItems();
    case "POST":
      return await createItem(JSON.parse(event.body));
    case "PUT":
      return await updateLoveCount(JSON.parse(event.body));
    default:
      return {
        statusCode: 400,
        body: JSON.stringify({ message: "Unsupported method" }),
      };
  }
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
