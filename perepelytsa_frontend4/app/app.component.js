"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
var core_1 = require('@angular/core');
var cart_component_1 = require('./cart.component');
var table_component_1 = require('./table.component');
var promo_component_1 = require('./promo.component');
var item_service_1 = require('./item.service');
var AppComponent = (function () {
    function AppComponent(itemService) {
        this.itemService = itemService;
    }
    AppComponent.prototype.getItems = function () {
        this.items = this.itemService.getItems();
    };
    AppComponent.prototype.ngOnInit = function () {
        this.getItems();
    };
    AppComponent = __decorate([
        core_1.Component({
            selector: 'my-app',
            template: "\n  <nav class=\"navbar navbar-default\">\n    \n <div class=\"container-fluid\">\n\n        <div class=\"navbar-header\">\n            <a class=\"navbar-brand \"  >SportStore</a>\n        </div>\n        <ul class=\"nav navbar-nav\">\n         \n        </ul>\n        <ul class=\"nav navbar-nav navbar-right\">\n            <li><a href=\"#\"><span class=\"glyphicon glyphicon-usd\"></span></a></li>\n            <li><a href=\"#\"><span class=\"glyphicon glyphicon-user\"></span></a></li>\n            <li><a href=\"#\"><span class=\"glyphicon glyphicon-envelope\"></span></a></li>\n\t\t\t<li id=\"clearcart\"><a href=\"#\"><span class=\"glyphicon glyphicon-remove\"></span></a></li>\n            <li><a href=\"shoppingcart.html\"><span class=\"glyphicon glyphicon-shopping-cart\"></span>\n\n<my-cart-value [itemsinp]=items></my-cart-value>\n\n </a></li></ul>\t\t\n    </div>\n\n\n    <div class=\"container-fluid lower-menu\">\n        <ul class=\"nav navbar-nav\">\n\t\t\n\t\t\t<li class=\"dropdown\">\n                <a class=\"dropdown-toggle\" data-toggle=\"dropdown\" href=\"#\">Bicycles\n                    <span class=\"caret\"></span></a>\n                <ul class=\"dropdown-menu\">\n                    <li><a href=\"#\">Utility bicycles</a></li>\n                    <li><a href=\"#\">Mountain bicycles</a></li>\n                    <li><a href=\"#\">Racing bicycles</a></li>\n                </ul>\n            </li>\n            <li class=\"dropdown\">\n                <a class=\"dropdown-toggle\" data-toggle=\"dropdown\" href=\"#\">Accesories\n                    <span class=\"caret\"></span></a>\n                <ul class=\"dropdown-menu\">\n                    <li><a href=\"#\">Bells</a></li>\n                    <li><a href=\"#\">Covers</a></li>\n                    <li><a href=\"#\">Mirrors</a></li>\n\t\t\t\t\t<li><a href=\"#\">Pumps</a></li>\t\t\t\t\t\n                </ul>\n            </li>\n            <li class=\"dropdown\">\n                <a class=\"dropdown-toggle\" data-toggle=\"dropdown\" href=\"#\">Spare parts\n                    <span class=\"caret\"></span></a>\n                <ul class=\"dropdown-menu\">\n                    <li><a href=\"#\">Brakes</a></li>\n                    <li><a href=\"#\">Cables</a></li>\n                    <li><a href=\"#\">Pedals</a></li>\n\t\t\t\t\t<li><a href=\"#\">Tires</a></li>\n\t\t\t\t\t<li><a href=\"#\">Chains</a></li>\n                </ul>\n            </li>\n        </ul>\n    </div>\n</nav>\n\n<div class = \"container-fluid bg-2\"> \n  <div class =\"container\"> \n      <div class=\"col-sm-12\">\n  <h2 class=\"text-center\">Shopping Cart</h2> \n  \n  \n    <my-table [itemsinp]=items></my-table>\n\n</div>\n<div class=\"col-sm-12 bg-9\">\n<my-promo [itemsinp]=items></my-promo>\n\n</div>\n   </div>\n   </div>\n   ",
            directives: [cart_component_1.CartComponent, table_component_1.TableComponent, promo_component_1.PromoComponent],
            providers: [item_service_1.ItemService]
        }), 
        __metadata('design:paramtypes', [item_service_1.ItemService])
    ], AppComponent);
    return AppComponent;
}());
exports.AppComponent = AppComponent;
//# sourceMappingURL=app.component.js.map