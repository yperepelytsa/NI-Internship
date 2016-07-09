
    $(document).ready(function(){
		shipToUkr();
        var i=0;
        $("#addtocart").click(function(){
			i+=parseInt($("#quant option:selected" ).text());
		if (i>4){
            alert("Don't be greedy! First pay!");
		}
		else{
			$("#shopping-cart-badge").css("visibility","visible");

            $("#shopping-cart-badge").text(i);
		}
		
        });
		$("#clearcart").click(function(){
			$("#shopping-cart-badge").text(0);
		});
		
		$(".image-preview").click(function(){
            var img=$(this).attr("src");
          $("#mainimg").attr("src", img);
        });
		
		$("#shipto").change(shipToUkr);
		
		$(" .buyquant").change(function(){
			var i=0;
			var deals=$(".sale");
			for(var j=0; j<deals.length; j++){
				i+=parseInt($(deals[j]).find("#price").text())*parseInt($(deals[j]).find("option:selected" ).text());
			}
			$("#summary").text(i);
		});
		
		function shipToUkr(){		
			var x = $("#shipto option:selected").text();
			var imgs=$(".col-sm-9 .bg-4");
		if(x=="Ukraine"){
			for(var i=0; i<imgs.length; i=i+2){
				imgs[i].textContent="Shipping to Ukraine";
			}
		}else{
			for(var i=0; i<imgs.length; i=i+2){
			imgs[i].textContent="HTX Actinum 5701";
			}
		}
			}
		
		
		
		 $(".image-sidemen").hover(function (e) {
			 $(e.target).attr("src","http://bikecolony.com/sites/default/files/big-nine-carbon-team-2013-2_1.jpg");
			 },function(e){
			$(e.target).attr("src","http://www.uabike.com/assets/images/article/news/2013-merida-mountain-bikes-add-new-vpk-suspension/2013-Merida-Ninety-Nine-29er-race-mountain-bike-600x376.jpg"); 				 
			 });
			 
		$("svg").click(function(){
		$("svg").find( "circle" ).attr("stroke-width","1");	
		$("svg").find( "circle" ).attr("fill","white");		
		$(this).find( "circle" ).attr("stroke-width","2");
		$(this).find( "circle" ).attr("fill","#ffffbb");	
		});	 
		
		//last task, animation on shopping cart badge
		setInterval(function(){
			if(i!=0){
			$("#shopping-cart-badge").animate({
				'opacity':0
			},"slow");
			setTimeout(function() {
				$("#shopping-cart-badge").animate({
				'opacity':1
			},"slow");
			},2500);
			}
			
		},5000);	 
		//	 
			 
			 
			 
		setInterval(function(){
			
		$('.sidetext').fadeOut(500, function() {
        $('.sidetext').text('1000$').fadeIn(500);
    });
		setTimeout(function() {
		$('.sidetext').fadeOut(500, function() {
        $('.sidetext').text('HTX Actinum 5702').fadeIn(500);
    });
		},5000);
		
    },10000);	 
		
	});
	




