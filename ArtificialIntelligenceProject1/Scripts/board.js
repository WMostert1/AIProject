function setupBoard() {
    var board_width = $("#board_wrapper").css("width");
    $("#board_wrapper").css("height", board_width);

    

    var total = $("#board_wrapper").children().length;
    var length = parseInt(parseInt(board_width) / Math.sqrt(total));
   
    for (var x = 0; x < total / 2; x++) {
        for (var y = 0; y < total / 2 ; y++) {
            $("#x" + x +"y"+ y).css("top", y*0.6*length + "px");
            $("#x" + x +"y"+ y).css("left", x * length + "px");
            $("#x" + x +"y"+ y).css("width", length + "px");
            $("#x" + x + "y" + y).css("height", 0.6 * length + "px");
            //$("#x" + x + "y" + y).click(function () {
            //    var coords = $(this).attr("id");
            //    coords = coords.split("y");
            //    coords[0] = coords[0].replace("x", "");
                
            //    $("#board_wrapper").load("Click", { x: coords[0], y: coords[1] });
            //});
        }
    }
}

function waitTurn() {
    $.get("waitTurn", function (result) {
        if (result == false) {
            $("#board_wrapper").load("currentGame");
        } else {
            var total = $("#board_wrapper").children().length;
            for (var x = 0; x < total / 2; x++) {
                for (var y = 0; y < total / 2 ; y++) {
                    $("#x" + x + "y" + y).click(function () {
                        var coords = $(this).attr("id");
                        coords = coords.split("y");
                        coords[0] = coords[0].replace("x", "");

                        $("#board_wrapper").load("Click", { x: coords[0], y: coords[1] });
                    });
                }
            }
        }
    });
}

