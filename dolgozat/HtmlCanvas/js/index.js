const myCanvas = document.getElementById("genCanvas");
const ctx = myCanvas.getContext("2d");
ctx.moveTo(1000,500);
class Node{
    constructor(xpos,ypos,branches,level,connectedfrom,parent){
        this.x = xpos;
        this.y = ypos;
        this.branches = branches;
        this.level = level;
        this.connectedFrom = connectedfrom;
        this.parentNodeIndex = parent;
    }
}
class Edge{
    constructor(from,to,lanes,oneway){
        this.from = from;
        this.to = to;
        this.lanes = lanes;
        this.oneway = oneway;
    }
}
class Graph{
    constructor(){
        this.Nodes = [];
        this.Edges = [];
    }
    drawAllNodes(){
        this.Nodes.forEach(node =>{
            drawCanvasNode(node);
        });
    }
    drawAllEdges(){
        this.Edges.forEach(edge =>{
            if(edge.oneway === false){
                drawCanvasEdge(edge);
            }
            else{
                console.log("123");
                canvas_arrow(edge);
            }
        });
    }
}
$(document).ready(

    DrawOnCanvas()
);

function DrawOnCanvas(){
    let graph = new Graph();
    var maxNodes = 50;
    var startNode = {x:1000,y:500,branches:4,level:0, connectedFrom:"none",parent: "none"};
    graph.Nodes.push(startNode);
    for(let i = 0;graph.Nodes.length < maxNodes;i++){
        let currentNode = graph.Nodes[i];
        let didEast = false;
        let didWest = false;
        let didNorth = false;
        let didSouth = false;
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

        for(let j = 0;j<currentNode.branches;j++){
            let direction;
            while(true){
                direction = Math.floor(Math.random()*4);
                if(direction === 0 && didSouth === false){
                    didSouth = true;
                    direction = "south";
                    break;
                }
                if(direction === 1 && didNorth === false){
                    didNorth = true;
                    direction = "north";
                    break;
                }
                if(direction === 2 && didWest === false){
                    didWest = true;
                    direction = "west";
                    break;
                }
                if(direction === 3 && didEast === false) {
                    didEast = true;
                    direction = "east";
                    break;
                }
            }
            let childConnectedFrom;
            let xpos, ypos;
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
            let branches = getBranchCount(currentNode.level+1);
            let lanes;
            if(branches === 3){
                lanes = 2;
            }else{
                lanes = 1;
            }
            let numedges = graph.Edges.length;
            let invalidedge = false;
            let corrected = false;
            let newNode = new Node(xpos,ypos,branches,currentNode.level + 1,childConnectedFrom,i);
            let newEdge = new Edge(currentNode,newNode,lanes,false);
                for (let k = 0; k < numedges; k++) {
                    if (numedges !== 0) {
                        let currentEdge = graph.Edges[k];
                        invalidedge = intersects(currentEdge,newEdge);
                        if(invalidedge && !corrected){
                            if(distance(newNode,currentEdge.from) > distance(newNode,currentEdge.to)){
                                if(!maxBranchesReached(currentEdge.to,graph,i)){
                                    newEdge.to = currentEdge.to;
                                    invalidedge = false;
                                    corrected = true;
                                }else{
                                    invalidedge = true;
                                    break;
                                }
                            }
                            else{
                                if(!maxBranchesReached(currentEdge.from,graph,i)) {
                                    newEdge.to = currentEdge.from;
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
                if(!invalidedge){
                    if(!corrected){
                        graph.Nodes.push(newNode);
                        graph.Edges.push(new Edge(currentNode,newNode,lanes,false));
                    }else{
                        newEdge.oneway = true;
                        graph.Edges.push(newEdge);
                    }
                }
        }
    }
    graph.drawAllNodes();
    graph.drawAllEdges();
    makeBusStops(graph);
}
function intersects(edge1,edge2) {
    let det, gamma, lambda;
    det = (edge1.to.x - edge1.from.x) * (edge2.to.y - edge2.from.y) - (edge2.to.x - edge2.from.x) * (edge1.to.y - edge1.from.y);
    if (det === 0) {
        return false;
    } else {
        lambda = ((edge2.to.y - edge2.from.y) * (edge2.to.x - edge1.from.x) + (edge2.from.x - edge2.to.x) * (edge2.to.y - edge1.from.y)) / det;
        gamma = ((edge1.from.y - edge1.to.y) * (edge2.to.x - edge1.from.x) + (edge1.to.x - edge1.from.x) * (edge2.to.y - edge1.from.y)) / det;
        return (0 < lambda && lambda < 1) && (0 < gamma && gamma < 1);
    }
}
function distance(node1,node2){
    return Math.sqrt(Math.pow((node1.x-node2.x),2) + Math.pow((node1.y-node2.y),2));
}
function maxBranchesReached(node,graph,currentNodeIndex) {
    for (let i = 0; i < graph.Nodes.length; i++) {
        if (graph.Nodes[i].x === node.x && graph.Nodes[i].y === node.y && i < currentNodeIndex) {
            if (graph.Nodes[i].branches < 3) {
                graph.Nodes[i].branches++;
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
function canvas_arrow(edge){
    const headlen = 10;   // length of head in pixels
    const angle = Math.atan2(edge.to.y - edge.from.y, edge.to.x - edge.from.x);
    ctx.beginPath();
    ctx.strokeStyle = "#2f40ff";
    ctx.moveTo(edge.from.x, edge.from.y);
    ctx.lineTo(edge.to.x, edge.to.y);
    ctx.lineTo(edge.to.x-headlen*Math.cos(angle-Math.PI/6),edge.to.y-headlen*Math.sin(angle-Math.PI/6));
    ctx.moveTo(edge.to.x, edge.to.y);
    ctx.lineTo(edge.to.x-headlen*Math.cos(angle+Math.PI/6),edge.to.y-headlen*Math.sin(angle+Math.PI/6));
    ctx.stroke();
    ctx.closePath();
}
function makeBusStops(graph){
    let potentialNodes = findMaxLevelNodes(graph);
    let start = potentialNodes[Math.floor(Math.random()*potentialNodes.length)];
    DrawBStops(start);
    var end = potentialNodes[0];
    for(let i = 0;i<potentialNodes.length;i++){
        if(distance(start.x,start.y,potentialNodes[i].x,potentialNodes[i].y)>distance(start.x,start.y,end.x,end.y)){
            end = potentialNodes[i];
        }
    }
    DrawBStops(end);
    var i = start.level;
    var currentNode = start;

    while(i !== 0){
        let nextStop = Math.floor(Math.random()*2)+1;
        for(let j = 0;j<nextStop;j++){
            currentNode = graph.Nodes[currentNode.parentNodeIndex];
        }
        if(currentNode === undefined){
            break;
        }
        DrawBStops(currentNode);
        i = currentNode.level;
    }
    i = end.level;
    currentNode = end;
    while(i !== 0){
        let nextStop = Math.floor(Math.random()*2)+1;
        for(let j = 0;j<nextStop;j++){
            currentNode = graph.Nodes[currentNode.parentNodeIndex];
        }
        if(currentNode === undefined){
            break;
        }
        DrawBStops(currentNode);
        i = currentNode.level;
    }
}
function DrawBStops(node){
    ctx.beginPath();
    ctx.strokeStyle = "#12ff03";
    ctx.strokeRect(node.x,node.y,10,10);
    ctx.closePath();
}
function findMaxLevelNodes(graph) {
    let max = graph.Nodes[0].level;
    let maxLevelNodes = [];
    for(let i = 0;i<graph.Nodes.length;i++){
        if(graph.Nodes[i].level > max){
            max = graph.Nodes[i].level;
        }
    }
    for(let i = 0;i<graph.Nodes.length;i++){
        if(graph.Nodes[i].level === max){
            maxLevelNodes.push(graph.Nodes[i]);
        }
    }
    return maxLevelNodes;
}
function drawCanvasNode(node){
    ctx.fillRect(node.x, node.y, 5, 5);
}
function drawCanvasEdge(edge){
    ctx.beginPath();
    ctx.moveTo(edge.from.x, edge.from.y);
    ctx.lineTo(edge.to.x, edge.to.y);
    ctx.strokeStyle = "#000000";
    ctx.stroke();
    ctx.closePath();
    ctx.moveTo(edge.from.x, edge.from.y);
}