﻿/*
 * Copyright (c) 2010-2012, Achim 'ahzf' Friedland <code@ahzf.de>
 * This file is part of Aegir <http://www.github.com/Vanaheimr/Aegir>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#region Usings

using System;
using System.Windows.Shapes;
using System.Windows.Media;

using de.ahzf.Vanaheimr.Aegir;
using de.ahzf.Illias.Commons;
using System.Windows;

#endregion

namespace de.ahzf.Vanaheimr.Aegir
{

    public static partial class GermanyPaths
    {
        public static String SachsenAnhalt = "M 97 4 L 98 6 100 6 101 5 102 5 102 8 103 9 103 10 105 10 106 9 107 8 108 8 109 8 109 8 110 9 111 10 111 11 112 12 112 12 116 13 116 13 117 14 117 14 117 16 116 16 115 18 115 19 117 20 119 20 120 20 121 22 122 22 122 23 125 23 127 24 128 24 129 25 130 25 131 25 132 25 132 23 133 23 134 23 134 23 138 24 138 24 139 24 139 25 139 25 140 25 141 25 141 25 143 23 143 25 143 26 144 27 143 28 142 29 143 29 144 28 145 28 146 28 147 27 148 27 149 25 149 25 150 26 150 27 150 27 150 27 151 28 151 27 151 27 151 27 153 28 153 29 153 30 153 31 154 34 154 35 155 35 155 37 154 37 154 38 154 38 154 39 153 38 152 38 152 41 151 41 151 42 151 42 150 42 150 43 151 44 151 44 151 45 152 46 151 47 151 47 150 47 150 48 149 48 149 49 150 49 151 49 151 49 150 49 150 50 151 51 151 51 152 51 152 52 151 52 151 52 152 53 153 53 153 54 152 54 152 54 153 54 153 56 153 56 153 57 153 58 153 59 153 60 153 61 153 61 153 62 153 62 152 63 150 63 150 63 150 64 149 64 147 63 147 66 147 66 147 70 148 71 146 72 146 73 145 75 145 76 145 78 146 77 146 77 147 76 147 76 148 77 148 77 148 78 147 79 147 81 147 81 148 81 148 82 149 82 149 82 149 82 150 82 151 82 151 81 151 81 151 82 152 82 152 81 152 81 152 80 152 79 152 79 152 79 153 79 152 78 153 78 155 79 155 79 155 81 156 81 156 82 156 82 156 82 156 83 157 83 157 83 158 83 158 82 159 82 159 83 160 83 159 84 159 85 160 85 161 85 161 85 162 85 161 86 161 87 160 87 160 88 160 89 159 89 159 90 159 91 159 92 158 92 158 92 158 93 157 94 156 95 158 95 158 95 158 96 159 95 159 96 159 97 159 97 158 98 158 100 159 100 159 101 159 101 157 101 157 102 159 103 159 104 159 104 159 105 160 105 159 105 158 108 157 108 156 110 156 111 156 112 155 113 155 113 154 115 154 117 154 118 154 119 154 119 156 119 155 120 156 120 156 120 157 121 157 121 158 122 158 122 158 122 158 123 157 124 155 124 154 125 154 125 154 125 154 125 154 126 154 127 154 129 153 129 152 128 151 130 151 131 151 131 151 131 153 132 153 133 153 133 153 134 154 135 156 137 156 138 156 139 156 140 157 140 157 141 158 141 158 141 159 141 159 142 160 142 161 142 161 144 161 145 162 146 162 146 163 147 164 149 164 149 167 149 168 150 169 151 170 152 170 153 170 153 173 153 173 152 173 151 174 150 174 150 175 151 175 152 176 153 176 154 176 154 177 154 178 155 180 155 180 156 181 157 181 158 188 158 188 158 187 157 188 157 190 157 191 155 191 154 193 154 194 155 194 155 196 155 196 156 196 156 197 156 198 157 201 158 201 159 202 159 202 161 203 161 203 161 206 161 207 161 208 160 209 160 209 162 209 162 209 165 213 165 213 166 214 167 215 166 215 164 216 164 218 165 219 165 219 167 219 167 221 167 221 168 220 170 224 170 225 171 225 170 227 170 227 172 226 172 227 173 226 173 226 173 226 174 226 174 228 174 228 175 231 175 231 174 233 174 234 174 234 173 235 173 236 174 237 174 237 176 235 176 235 177 234 176 234 178 235 180 235 180 236 181 236 181 237 183 238 185 238 185 237 186 238 186 237 187 238 188 237 189 237 189 237 189 237 190 237 191 237 191 238 192 238 192 237 192 237 193 238 194 238 194 238 194 239 195 239 195 240 196 240 197 238 197 239 198 239 199 238 198 237 198 237 199 238 200 238 200 238 201 237 201 228 207 225 209 225 208 225 206 224 205 223 205 223 204 221 204 221 205 222 205 221 205 221 205 220 205 220 207 220 206 219 207 218 207 219 208 218 209 218 209 218 208 217 207 216 207 216 207 216 208 215 208 214 207 213 207 213 206 213 206 213 205 213 205 212 206 211 206 211 206 211 206 211 205 210 205 210 204 211 203 210 203 209 202 207 202 207 203 207 204 206 204 206 204 205 204 205 205 204 205 203 207 202 208 201 208 201 207 200 207 200 206 199 206 199 206 199 206 197 205 194 205 194 206 194 206 194 207 192 208 192 210 191 210 191 211 191 211 190 212 189 212 189 212 189 211 189 210 189 211 186 211 186 211 186 212 185 212 185 211 185 210 184 212 184 212 183 213 182 213 181 213 181 214 181 214 180 214 179 214 178 213 178 214 176 215 175 215 175 215 176 214 176 214 174 214 174 214 174 214 173 213 173 213 172 213 172 213 171 213 172 214 172 215 170 215 170 217 168 217 167 216 166 217 165 217 163 216 163 217 162 217 163 217 162 217 162 218 161 218 161 218 160 219 159 219 158 220 158 220 154 220 154 219 153 219 153 220 153 221 153 221 153 222 151 223 151 224 150 224 150 224 149 224 149 225 149 225 149 226 149 226 149 228 149 228 149 228 150 229 150 229 150 229 150 231 148 231 148 233 147 234 147 233 146 234 145 234 145 235 146 236 146 238 147 238 147 239 147 238 148 239 149 241 149 240 149 240 149 241 148 241 148 242 148 242 148 243 148 243 148 245 149 245 148 245 147 247 148 247 148 248 149 248 149 249 149 249 149 249 150 249 150 250 149 251 149 251 149 253 148 254 147 254 147 255 145 255 146 257 146 258 146 258 146 259 147 259 147 260 147 261 147 262 147 262 147 262 147 264 148 264 149 264 149 265 149 269 150 270 150 270 149 272 149 273 148 273 149 273 148 274 148 274 148 275 149 274 150 274 151 275 151 276 150 276 150 278 150 277 150 281 151 281 153 281 154 282 154 283 152 283 152 283 152 285 152 285 152 286 153 286 154 287 155 287 155 287 156 287 157 287 157 287 158 288 158 289 157 289 156 290 156 291 156 291 156 294 155 295 154 295 154 295 155 296 156 296 156 295 158 295 158 296 159 296 159 298 158 298 158 299 158 299 158 300 157 300 157 301 156 302 156 302 155 303 155 303 154 304 154 305 153 306 153 306 154 306 154 308 153 308 153 308 153 309 152 309 152 309 152 309 151 308 152 308 152 307 151 307 151 307 150 307 150 307 149 306 149 306 148 304 147 306 147 307 146 306 145 306 145 306 144 307 144 306 143 306 142 305 142 305 141 305 140 305 140 304 140 304 139 304 138 304 137 306 136 305 135 305 133 305 133 305 133 304 133 304 133 303 133 303 132 303 132 302 132 302 132 302 130 302 129 302 129 302 129 302 129 301 129 301 129 299 130 299 129 299 128 298 128 298 126 296 126 297 126 296 125 297 123 297 124 296 123 296 123 296 124 295 123 295 123 295 123 294 122 295 122 294 121 293 120 293 119 294 118 294 117 293 117 294 116 294 115 294 113 294 112 294 111 294 110 294 110 295 110 295 109 295 109 294 109 294 109 294 108 294 108 293 109 293 109 293 108 292 107 292 107 291 106 291 106 291 105 291 105 291 104 291 104 291 104 288 103 288 103 287 102 287 102 286 101 286 101 286 101 286 101 285 100 286 99 286 98 285 97 286 97 287 96 287 96 286 95 284 94 284 92 284 92 284 91 285 91 286 90 286 90 285 90 285 90 287 89 286 88 286 86 286 86 287 84 287 84 286 84 286 83 286 82 285 82 282 82 282 82 281 81 281 81 280 83 280 84 279 83 278 83 277 84 277 84 277 83 276 83 274 81 273 81 273 81 271 80 270 79 270 77 271 77 272 76 272 75 271 75 270 76 270 75 270 75 269 73 269 73 268 74 268 74 267 75 267 75 266 75 266 75 266 76 266 76 267 77 267 77 267 77 266 78 266 79 265 79 265 80 264 80 263 80 262 80 262 81 262 81 261 81 260 82 259 82 259 82 260 83 259 83 258 82 257 81 256 81 255 80 255 80 255 79 254 78 253 78 253 79 253 79 252 76 252 76 251 76 251 76 249 75 249 76 248 75 248 75 247 76 247 76 246 75 246 74 245 74 246 74 245 73 246 73 246 73 246 73 245 72 245 72 245 72 246 71 245 71 245 70 245 70 243 69 242 69 242 68 242 66 243 66 243 65 242 65 243 63 243 62 244 62 242 61 242 61 242 60 241 60 241 59 242 57 242 57 243 57 243 57 242 55 242 55 243 53 243 52 242 52 242 52 241 51 242 50 242 50 241 49 241 49 240 48 240 47 239 44 239 44 240 43 240 42 241 39 241 40 240 41 240 41 239 40 239 40 240 38 240 38 239 37 239 37 237 37 237 37 237 37 236 37 235 37 235 37 235 36 235 37 234 36 233 36 233 37 232 37 232 37 232 37 231 36 230 36 230 36 229 35 229 35 229 34 229 34 228 35 228 35 227 34 226 34 224 34 223 35 223 35 224 35 224 35 223 35 223 34 223 34 222 33 222 33 222 32 222 32 221 31 221 31 222 30 221 30 218 29 217 30 215 30 215 31 215 34 215 34 215 34 213 33 213 32 213 33 211 32 211 31 213 30 213 30 212 29 212 28 210 27 209 26 209 24 211 23 210 23 208 20 208 19 208 19 207 18 207 17 208 18 208 17 208 15 207 15 207 13 208 13 208 11 206 11 206 11 205 10 204 10 204 10 201 9 200 10 200 10 200 10 198 10 198 9 196 8 197 8 196 7 195 7 194 7 194 6 193 6 191 6 190 5 190 4 190 4 189 2 190 2 188 1 187 2 187 2 184 1 183 1 180 2 180 2 179 2 178 1 178 1 177 2 177 3 176 4 176 4 175 5 174 6 174 6 173 8 172 8 170 8 169 8 169 8 168 7 168 7 167 6 167 4 166 5 165 5 164 4 164 4 164 6 162 7 162 8 162 8 161 8 161 7 160 7 161 5 160 5 160 6 160 6 159 5 159 4 159 4 158 3 158 3 158 2 158 2 157 0 154 1 154 1 153 3 153 3 152 3 152 3 153 4 153 4 153 5 153 6 153 6 153 6 153 7 153 7 152 7 151 8 151 8 151 9 151 9 150 8 149 8 148 10 148 12 147 27 148 27 147 29 147 30 146 36 146 37 147 37 147 37 146 38 145 38 142 37 142 36 142 35 141 35 140 36 140 37 139 38 140 38 140 39 139 41 138 42 138 41 137 42 136 43 136 44 135 44 135 44 134 45 133 45 133 46 133 45 132 46 131 46 131 45 130 43 130 42 129 42 129 42 126 43 126 42 126 42 125 43 124 44 124 44 123 45 123 47 123 47 122 48 122 48 121 47 120 47 120 45 120 45 118 45 117 45 116 45 116 44 115 43 115 42 114 42 114 41 112 41 112 42 111 43 111 44 111 44 110 43 110 42 108 41 107 41 107 40 106 39 106 39 105 39 105 40 105 41 104 42 104 43 105 44 104 45 104 46 103 46 100 47 100 47 100 46 100 46 100 46 99 45 99 45 99 45 98 43 98 42 97 42 96 40 94 39 94 39 93 37 92 37 91 36 91 36 89 35 89 35 88 34 87 34 85 34 85 34 84 35 83 34 83 35 82 36 82 36 82 37 81 38 81 38 82 40 82 40 82 41 82 41 82 38 79 36 77 36 76 35 75 35 74 34 73 34 72 34 71 34 71 34 69 35 69 35 69 34 68 35 68 36 67 36 67 37 66 37 66 38 65 38 63 37 63 35 63 34 64 34 64 33 65 33 65 33 64 32 64 31 63 31 62 31 62 31 61 30 60 29 58 29 56 28 56 28 55 28 55 28 55 27 55 27 53 26 53 26 52 26 51 25 51 24 50 24 50 25 49 25 49 24 48 24 47 22 47 22 48 21 48 21 45 21 45 21 43 21 42 21 42 21 41 20 41 20 40 19 39 19 39 18 38 18 38 18 37 18 35 19 34 18 33 18 31 18 30 19 30 19 29 18 28 19 28 21 28 22 28 22 27 23 28 25 28 26 27 27 28 27 27 28 27 28 28 29 28 29 28 30 28 31 27 33 28 35 28 35 27 36 26 36 25 37 24 38 24 38 23 39 23 39 20 40 20 40 19 44 19 44 19 44 19 45 19 46 19 47 19 49 19 49 21 50 22 51 22 53 21 54 21 54 21 54 20 57 21 60 21 60 22 61 22 62 23 62 24 65 24 65 24 66 23 67 23 67 24 67 25 69 25 71 23 72 23 74 22 75 21 77 21 77 20 78 20 79 19 79 18 80 18 81 17 83 15 84 15 85 15 86 15 85 13 85 10 86 9 86 8 87 7 87 6 87 5 88 5 89 6 89 6 90 7 91 7 91 5 91 5 91 5 91 4 92 4 92 4 92 4 95 2 95 1 97 0 98 0 99 1 98 2 97 3 z";
    }

    /// <summary>
    /// A feature on an Aegir map.
    /// </summary>
    public class SachsenAnhalt : AShape
    {

        #region SachsenAnhalt(StrokeColor, StrokeThickness, FillColor)

        public SachsenAnhalt(Color StrokeColor, Double StrokeThickness, Color FillColor)
            : base(GermanyPaths.SachsenAnhalt, 53.038791, 10.563370, 0, 50.945049, 13.199229, StrokeColor, StrokeThickness, FillColor)
        { }

        #endregion

    }

}

