var connection = new WebSocket('ws://localhost:8958');
var app = null;

window.addEventListener('DOMContentLoaded', function() {
  app = document.querySelector('#app');

  app.data = {
    PositionInformation: {
      Latitude: 0,
      Longitude: 0,
      Altitude: 0
    }
  };

  connection.onmessage = function(e) {
    app.data = JSON.parse(e.data);
    console.log(app.data);
  };
});
