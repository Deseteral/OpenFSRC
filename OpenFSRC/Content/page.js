var connection = new WebSocket('ws://localhost:8958');

connection.onmessage = function(e) {
  var data = JSON.parse(e.data);
  console.log(data);
};
