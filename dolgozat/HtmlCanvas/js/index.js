const Nodes = [];
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
    Nodes.push(startNode);
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
            var branches = getBranchCount(currentNode.level+1);
            var lanes;
            if(branches == 3){
                lanes = 2;
            }else{
                lanes = 1;
            }
            let numedges = Edges.length;
            var invalidedge = false;
            var corrected = false;
                for (var k = 0; k < numedges; k++) {
                    if (numedges != 0) {
                        var currentEdge = Edges[k];
                        invalidedge = intersects(currentEdge.from.x,currentEdge.from.y,currentEdge.to.x,currentEdge.to.y,currentNode.x,currentNode.y,xpos,ypos);
                        if(invalidedge && !corrected){
                            if(distance(xpos,ypos,currentEdge.from.x,currentEdge.from.y) > distance(xpos,ypos,currentEdge.to.x,currentEdge.to.y)){
                                if(!maxBranchesReached(currentEdge.to.x,currentEdge.to.y,i)){
                                    xpos = currentEdge.to.x;
                                    ypos = currentEdge.to.y;
                                    invalidedge = false;
                                    corrected = true;
                                }else{
                                    invalidedge = true;
                                    break;
                                }


                            }
                            else{
                                if(!maxBranchesReached(currentEdge.from.x,currentEdge.from.y,i)) {
                                    xpos = currentEdge.from.x;
                                    ypos = currentEdge.from.y;
                                    invalidedge = false;
                                    corrected = true;
                                }else{
                                    invalidedge = true;
                                    break;
                                }
                            }

                        }else if(invalidedge){
                            break;
                        }
                    }
                }
                console.log(branches);
                if(!invalidedge){
                    if(!corrected){
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
                            lanes: lanes,
                            oneway: false
                        });
                        ctx.beginPath();
                        ctx.moveTo(currentNode.x, currentNode.y);
                        ctx.lineTo(xpos, ypos);
                        ctx.strokeStyle = "#000000";
                        ctx.stroke();
                        ctx.closePath();
                        ctx.moveTo(currentNode.x, currentNode.y);
                    }else{
                        console.log("changing color");

                        Edges.push({
                            from: {x: currentNode.x, y: currentNode.y},
                            to: {x: xpos, y: ypos},
                            lanes: lanes,
                            oneway: true
                        });
                        ctx.beginPath();
                        ctx.moveTo(currentNode.x, currentNode.y);
                        canvas_arrow(ctx,currentNode.x,currentNode.y,xpos,ypos);
                        ctx.strokeStyle = "#0000ff";
                        ctx.stroke();
                        ctx.closePath();
                        ctx.moveTo(currentNode.x, currentNode.y);
                    }



                }
        }
        i++;
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
}
function distance(a,b,c,d){
    return Math.sqrt(Math.pow((a-c),2) + Math.pow((b-d),2));
}
function maxBranchesReached(x,y,currentNodeIndex) {
    for (var i = 0; i < Nodes.length; i++) {
        if (Nodes[i].x == x && Nodes[i].y == y && i < currentNodeIndex) {
            if (Nodes[i].branches < 3) {
                console.log("drawing edge to node, new branch count is "+Nodes[i].branches);
                Nodes[i].branches++;
                return false;
            }
            else {
                return true;
            }
        }
    }
    return true;
}
function getBranchCount(level){
    var num = Math.floor(Math.random()*100)+1;
    if(num < 100-level*20){
        return 3;
    }else{
        return Math.floor((Math.random()*2)+1);
    }
}
function canvas_arrow(context, fromx, fromy, tox, toy){
    var headlen = 10;   // length of head in pixels
    var angle = Math.atan2(toy-fromy,tox-fromx);
    context.moveTo(fromx, fromy);
    context.lineTo(tox, toy);
    context.lineTo(tox-headlen*Math.cos(angle-Math.PI/6),toy-headlen*Math.sin(angle-Math.PI/6));
    context.moveTo(tox, toy);
    context.lineTo(tox-headlen*Math.cos(angle+Math.PI/6),toy-headlen*Math.sin(angle+Math.PI/6));
}
