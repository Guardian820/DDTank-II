function getCookie(objName){
   var arrStr = document.cookie.split("; ");
   for(var i = 0;i < arrStr.length;i ++){
    var temp = arrStr[i].split("=");
    if(temp[0] == objName) return unescape(temp[1]);
   } 
  }

var cookiename = 'societyguestname';
if(getCookie(cookiename)){
var user_info = {"status":true, "msg":getCookie(cookiename)};
}
else{
var user_info = {"status":false};
}
var rand = Math.random();
var head= document.getElementsByTagName("head")[0];
var script = document.createElement('script');
script.src = "#" + rand;
head.appendChild(script);