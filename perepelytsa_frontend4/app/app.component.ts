import { Component, OnInit } from '@angular/core';
import { CartComponent } from './cart.component';
import { TableComponent } from './table.component';
import { PromoComponent } from './promo.component';
import { Item } from './item';
import { ItemService } from './item.service';

@Component({ 
  selector: 'my-app',
  template: `
  <nav class="navbar navbar-default">
    
 <div class="container-fluid">

        <div class="navbar-header">
            <a class="navbar-brand "  >SportStore</a>
        </div>
        <ul class="nav navbar-nav">
         
        </ul>
        <ul class="nav navbar-nav navbar-right">
            <li><a href="#"><span class="glyphicon glyphicon-usd"></span></a></li>
            <li><a href="#"><span class="glyphicon glyphicon-user"></span></a></li>
            <li><a href="#"><span class="glyphicon glyphicon-envelope"></span></a></li>
			<li id="clearcart"><a href="#"><span class="glyphicon glyphicon-remove"></span></a></li>
            <li><a href="shoppingcart.html"><span class="glyphicon glyphicon-shopping-cart"></span>

<my-cart-value [itemsinp]=items></my-cart-value>

 </a></li></ul>		
    </div>


    <div class="container-fluid lower-menu">
        <ul class="nav navbar-nav">
		
			<li class="dropdown">
                <a class="dropdown-toggle" data-toggle="dropdown" href="#">Bicycles
                    <span class="caret"></span></a>
                <ul class="dropdown-menu">
                    <li><a href="#">Utility bicycles</a></li>
                    <li><a href="#">Mountain bicycles</a></li>
                    <li><a href="#">Racing bicycles</a></li>
                </ul>
            </li>
            <li class="dropdown">
                <a class="dropdown-toggle" data-toggle="dropdown" href="#">Accesories
                    <span class="caret"></span></a>
                <ul class="dropdown-menu">
                    <li><a href="#">Bells</a></li>
                    <li><a href="#">Covers</a></li>
                    <li><a href="#">Mirrors</a></li>
					<li><a href="#">Pumps</a></li>					
                </ul>
            </li>
            <li class="dropdown">
                <a class="dropdown-toggle" data-toggle="dropdown" href="#">Spare parts
                    <span class="caret"></span></a>
                <ul class="dropdown-menu">
                    <li><a href="#">Brakes</a></li>
                    <li><a href="#">Cables</a></li>
                    <li><a href="#">Pedals</a></li>
					<li><a href="#">Tires</a></li>
					<li><a href="#">Chains</a></li>
                </ul>
            </li>
        </ul>
    </div>
</nav>

<div class = "container-fluid bg-2"> 
  <div class ="container"> 
      <div class="col-sm-12">
  <h2 class="text-center">Shopping Cart</h2> 
  
  
    <my-table [itemsinp]=items></my-table>

</div>
<div class="col-sm-12 bg-9">
<my-promo [itemsinp]=items></my-promo>

</div>
   </div>
   </div>
   `,
   directives: [CartComponent,TableComponent,PromoComponent],
   providers: [ItemService]
})
export class AppComponent implements OnInit{ 
  items: Item[];

constructor(private itemService: ItemService){ }

getItems() {
    this.items=this.itemService.getItems()
}

ngOnInit(){
    this.getItems();
}

}

 


