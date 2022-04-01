require 'fileutils'
require 'pathname'
require 'webrick'
require 'find'


module Server
    class NonCachingFileHandler < WEBrick::HTTPServlet::FileHandler
      def initialize(server, root, options={}, default=WEBrick::Config::FileHandler)
        super(server, root, { **options, :FancyIndexing => true }, default)
      end

      def prevent_caching(res)
        res['ETag']          = nil
        res['Last-Modified'] = Time.now + 100**4
        res['Cache-Control'] = 'no-store, no-cache, must-revalidate, post-check=0, pre-check=0'
        res['Pragma']        = 'no-cache'
        res['Expires']       = Time.now - 100**4
      end

      def do_GET(req, res)
        super
        prevent_caching(res)
      end
    end


    def self.serve_dist
      server = WEBrick::HTTPServer.new :Port => 8000
      server.mount '/', NonCachingFileHandler , 'dist'
      trap 'INT' do server.shutdown end
      server.start
    end
  end



def is_root?(filename)
    %r{// ROOT} =~ IO.readlines(filename).first
end

def find_root_asciidocs_files
    Find.find('.').select do |filename|
        filename.end_with? '.asciidoc' and is_root? filename
    end
end

def compile_asciidoc
    puts "Compiling asciidocs..."
    roots = find_root_asciidocs_files
    `asciidoctor -R docs -D dist #{roots.join(' ')}`
end

def compile_graphviz
    root = Pathname.new 'docs'
    dist = Pathname.new 'dist'

    Find.find('docs') do |entry|
        if entry.end_with? '.gv'
            path = Pathname.new entry
            relative_path = path.relative_path_from root
            from_path = root + relative_path
            to_path = dist + relative_path
            to_path.dirname.mkpath

            puts "#{from_path} -> #{to_path}"
            output = `dot -Tsvg #{from_path.expand_path} -o #{to_path.expand_path.sub_ext('.svg')}`.strip
            puts output if output.size > 0
        end
    end
end

desc 'Removes dist directory'
task :clean do
    FileUtils.rm_rf 'dist'
end

namespace :build do
    desc 'Compiles asciidoc'
    task :asciidoc do
        compile_asciidoc
        compile_graphviz
    end

    desc 'Compiles graphviz'
    task :graphviz do
        compile_graphviz
    end

    desc 'Compiles everything'
    task :all => [ 'build:asciidoc', 'build:graphviz' ]
end

desc 'Uploads dist to server'
task :upload do
    Dir.chdir 'dist' do
        `ssh upload.leone.ucll.be rm -rf /home/frederic/courses/vgo/volume/*`
        puts `scp -r * upload.leone.ucll.be:/home/frederic/courses/vgo/volume`
    end
end

desc 'Equivalent to build:all'
task :default => [ "build:all" ]

desc 'Clean build (no upload)'
task :all => [ :clean, 'build:all' ]

desc 'Clean build + upload'
task :full => [ :all, :upload ]

desc 'Serves on localhost:8000'
task :serve do
    Server::serve_dist
end
