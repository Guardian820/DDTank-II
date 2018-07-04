// JavaScript Document

function CheckFlashVersion(v){
	for(var i = 0; i < bannedFlash.length; ++i){
		if(bannedFlash[i] == v){
			return -1;
		}
	}

	for(var i = 0; i < bannedFlash.length; ++i){
		if(parseInt(bannedFlash[i].split(',')[0]) > parseInt(v.split(',')[0])){
			return 1;
		}
		else if(parseInt(bannedFlash[i].split(',')[0]) == parseInt(v.split(',')[0])){
			if(parseInt(bannedFlash[i].split(',')[1]) > parseInt(v.split(',')[1])){
				return 1;
			}
			else if(parseInt(bannedFlash[i].split(',')[1]) == parseInt(v.split(',')[1])){
				if(parseInt(bannedFlash[i].split(',')[2]) > parseInt(v.split(',')[2])){
					return 1;
				}
				else if(parseInt(bannedFlash[i].split(',')[2]) == parseInt(v.split(',')[2])){
					if(parseInt(bannedFlash[i].split(',')[3]) > parseInt(v.split(',')[3])){
						return 1;
					}
				}
			}
		}
	}
	return 0;
}

function isSafeFlash(v){
	
	switch(CheckFlashVersion(v)){
		case -100://use to be case -1
			var div = document.createElement("div");
			div.style.width = parseInt(window.screen.width) + "px";
			div.style.height = parseInt(window.screen.height) * 0.85 + "px";
			div.style.top = 0;
			div.style.left = 0;
			div.style.position = "absolute";
			div.style.background = "white";
			document.body.style.background = "white";
			
			var centerDiv = document.createElement("div");
			centerDiv.style.width = "1024px";
			centerDiv.style.height = "650px";
			centerDiv.style.position = "absolute";
			centerDiv.style.top = (document.body.clientHeight - 650) / 2 + "px";
			centerDiv.style.left = (document.body.clientWidth - 1024) / 2 + "px";

			var centerIframe = document.createElement("iframe");
			centerIframe.setAttribute("frameborder","no");
			centerIframe.setAttribute("border","0");
			centerIframe.setAttribute("marginwidth","0");
			centerIframe.setAttribute("marginheight","0");
			centerIframe.style.width = "100%";
			centerIframe.style.height = "100%";
			centerIframe.style.position = "relative";
			centerIframe.src = "UpdateFlash.html";

			centerDiv.appendChild(centerIframe);
			document.body.appendChild(div);
			document.body.appendChild(centerDiv);
			break;
		case 0:
			break;
		case -1:
		case 1:
			if(confirm("Để chơi tốt hơn, hãy update phiên bản flash mới nhất.\nChọn “xác nhận” để rời game update.\nChọn “hủy” tiếp tục trò chơi。")){
				updatePlayer();
			};
			break;
	}
}
function updatePlayer(){
	//document.location = "http://www.adobe.com/go/getflashplayer"
	window.open("http://www.adobe.com/go/getflashplayer");
	
}
