
$(document).ready(
    DrawOnCanvas()
);
function DrawOnCanvas(){
    var myCanvas = document.getElementById("genCanvas");
    var ctx = myCanvas.getContext("2d");
    var maxBusStop = 0;
    var maxNodes = 50;
    ctx.moveTo(1000,500);
    var startNode = {x:1000,y:500,branches:4,level:0, connectedFrom:"none"};
    var Nodes = [startNode];
    var Edges =[];
    ctx.fillRect(1000,500,5,5);
    var i = 0;
    while(Nodes.length < maxNodes){
        var currentNode = Nodes[i];
        ctx.moveTo(currentNode.x,currentNode.y);
        var didEast = false;
        var didWest = false;
        var didNorth = false;
        var didSouth = false;
        switch (currentNode.connectedFrom) {
            case "south":
                didSouth = true;
                break;
            case "north":
                didNorth = true;
                break;
            case "west":
                didWest = true;
                break;
            case "east":
                didEast = true;
                break;
            default:
                console.log("No case match!");
        }

        for(j = 0;j<currentNode.branches;j++){
            var direction;
            while(true){
                direction = Math.floor(Math.random()*4);
                if(direction == 0 && didSouth == false){
                    didSouth = true;
                    direction = "south";
                    break;
                }
                if(direction == 1 && didNorth == false){
                    didNorth = true;
                    direction = "north";
                    break;
                }
                if(direction == 2 && didWest == false){
                    didWest = true;
                    direction = "west";
                    break;
                }
                if(direction == 3 && didEast == false) {
                    didEast = true;
                    direction = "east";
                    break;
                }
            }
            console.log("direction "+direction+j);
            var childConnectedFrom;
            var xpos, ypos;
            switch (direction) {
                case "south":
                    childConnectedFrom = "north";
                    xpos = Math.floor(Math.random()*20) - 10 + currentNode.x;
                    ypos = Math.floor(Math.random()*100) + 50 + currentNode.y;
                    break;
                case "north":
                    childConnectedFrom = "south";
                    xpos = Math.floor(Math.random()*20) - 10 + currentNode.x;
                    ypos = -(Math.floor(Math.random()*100) + 50) + currentNode.y;
                    break;
                case "west":
                    childConnectedFrom = "east";
                    xpos = -(Math.floor(Math.random()*100) + 50) + currentNode.x;
                    ypos = Math.floor(Math.random()*20) - 10 + currentNode.y;
                    break;
                case "east":
                    childConnectedFrom = "west";
                    xpos = Math.floor(Math.random()*100) + 50 + currentNode.x;
                    ypos = Math.floor(Math.random()*20) - 10 + currentNode.y;
                    break;
                default:
                    console.log("No case match!");
            }

            //80 60 40 20
            var branches = Math.floor(Math.random()*4);
            let numedges = Edges.length;
            var invalidedge = false;
                for (var k = 0; k < numedges; k++) {
                    if (numedges != 0) {
                        var currentEdge = Edges[k];
                        invalidedge = intersects(currentEdge.from.x,currentEdge.from.y,currentEdge.to.x,currentEdge.to.y,currentNode.x,currentNode.y,xpos,ypos);
                        if(invalidedge){
                            break;
                        }
                    }
                }
                if(!invalidedge){
                    Nodes.push({
                        x: xpos,
                        y: ypos,
                        branches: branches,
                        level: currentNode.level + 1,
                        connectedFrom: childConnectedFrom
                    });

                    ctx.fillRect(xpos, ypos, 5, 5);
                    Edges.push({
                        from: {x: currentNode.x, y: currentNode.y},
                        to: {x: xpos, y: ypos},
                        lanes: 1,
                        oneway: false
                    });
                    ctx.lineTo(xpos, ypos);
                    ctx.stroke();
                    ctx.moveTo(currentNode.x, currentNode.y);
                }
        }
        i++;
        console.log(i);
    }
}
function intersects(a,b,c,d,p,q,r,s) {
    var det, gamma, lambda;
    det = (c - a) * (s - q) - (r - p) * (d - b);
    if (det === 0) {
        return false;
    } else {
        lambda = ((s - q) * (r - a) + (p - r) * (s - b)) / det;
        gamma = ((b - d) * (r - a) + (c - a) * (s - b)) / det;
        return (0 < lambda && lambda < 1) && (0 < gamma && gamma < 1);
    }
};

