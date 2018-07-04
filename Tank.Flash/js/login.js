var v;
function clearTips(obj){
	v         = obj.value;
	obj.value = "";
}
function cheak(obj){
	var tipsValue = v;
	var thisValue = obj.value;
	if(obj.value==""||obj.value==tipsValue){
		obj.value = tipsValue;
		v,obj = ""
	}else{
		obj.value = thisValue;
		v,obj = ""
	}
}
if(user_info.status == true){
		document.write("<div id='haslogin'>");
		document.write("<ul style='padding-top: 0px;'>");
		document.write("<li>");
		document.write("<p style='text-align: center; color: #CD715A; display: block; font-size: 12px;padding: 14px;'><strong style='' id='showname'>"+ user_info.msg +"</strong></p>");
		document.write("</li>");
		document.write("</ul>");
		document.write("<div style='border-top: 1px dotted rgb(148, 170, 162); padding: 5px 6px 0pt;'>");
		document.write("<h2 style='color: rgb(178, 75, 0); font-size:12px'>Recommend:</h2>");
		document.write("<ul class='l-f-a'>");
		document.write("<li><span style='float:left'>Bazooka</span><a class='server_enter' href='#'></a></li>");
		document.write("</ul>");
		document.write("<p class='l-f-b'><a href='#'>Edit Profile</a><a id='loginout1' href='#'>Logout</a></p>");
		document.write("</div>");
		document.write("</div>");
}
else{
document.write("<div id='con_one_1'>");
document.write("<form method='post' action='#'>");
document.write("<input name='UserName' type='text' class='signBox' id='user_name' value='UserName' onfocus='clearTips(this)' onblur='cheak(this)' />");
document.write("<input name='Password' type='password' class='signBox' id='psw' value='PassWord' onfocus='clearTips(this)' onblur='cheak(this)' />");
document.write("<input name='' type='submit' class='loginBtn' value=''/>");
document.write("<div class='signBottom'><a target='_blank' href='#'>Forgot Password?</a>");
document.write("<a  href='#'><img src='http://ddt.hithere.com/public/images/sign_up.png' /></a></div>");
document.write("</form>");
document.write("</div>");						
}